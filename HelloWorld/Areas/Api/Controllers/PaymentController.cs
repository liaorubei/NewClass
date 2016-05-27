
using Com.Alipay;
using PayPal.Api;
using PayPal.Sample;
using StudyOnline.Models;
using StudyOnline.Utils;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace StudyOnline.Areas.Api.Controllers
{
    public class PaymentController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private StudyOnlineEntities entities = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult VerifyOrder(String orderId, Int32 mode, String result)
        {
            ActionResult q = null;
            switch (mode)
            {
                case 0:
                    q = VerifyAliPay(result, orderId);
                    break;
                case 1:
                    q = VerifyPayPal(result, orderId);
                    break;

                default:
                    q = Json(new { code = 201, desc = "请选择支付方式" });
                    break;
            }
            return q;
        }

        [HttpPost]
        public ActionResult CreateOrder(Orders order)
        {
            order.Id = Guid.NewGuid().ToString().Replace("-", "");
            order.CreateTime = DateTime.Now;

            if (!entities.NimUser.Any(o => o.Username == order.UserName))
            {
                return Json(new { code = 200, desc = "指定用户不存在", info = order });
            }

            //order = new StudyOnline.Models.Orders();
            //order.Id = "123456789";
            //order.Amount = 0.01;
            //order.Main = "ChineseChat充值";
            //order.Body = "ChineseChat充值1000学币";

            String orderString = OrderUtil.getOrderInfo(order);
            String sign = RSAFromPkcs8.sign(orderString, Config.Private_key, Config.Input_charset);

            //原始订单字符串+URL编码的签名+签名类型
            String lastOrderString = orderString + "&sign=\"" + HttpUtility.UrlEncode(sign, Encoding.UTF8) + "\"&sign_type=\"" + Config.Sign_type + "\"";

            entities.Orders.Add(order);
            entities.SaveChanges();
            return Json(new { code = 200, desc = "订单创建成功", info = new { order.Id, LastOrderString = lastOrderString } });
        }

        [HttpPost]
        public ActionResult VerifyAliPay(String orderId, String result)
        {
            Orders order = entities.Orders.Find(orderId);
            if (order == null)
            {
                return Json(new { code = 201, desc = "指定订单不存在" });
            }

            NimUser user = entities.NimUser.Single(o => o.Username == order.UserName);

            //如果异步通知成功,直接返回
            if (order.TradeStatus == "TRADE_SUCCESS" || order.TradeStatus == "TRADE_FINISHED")
            {
                //平衡学币
                user.NimUserEx.Coins = order.Coin + (user.NimUserEx.Coins ?? 0);
                order.IsBalance = 1;
                entities.SaveChanges();

                return Json(new { code = 200, desc = "支付成功", info = new { user.Username, Nickname = user.NimUserEx.Name, user.NimUserEx.Coins } });
            }

            //如果服务端没有收到异步通知的时候,则要验证客户端发过来的同步通知(https://doc.open.alipay.com/doc2/detail.htm?spm=0.0.0.0.bsvyrx&treeId=59&articleId=103665&docType=1)
            //1、原始数据是否跟商户请求支付的原始数据一致（必须验证这个）；
            //2、验证这个签名是否能通过。上述1、2通过后，在sign字段中success = true才是可信的。

            //构建原始数据,并验证是否一致,比如如果订单号不存在,那么就会验证不成功
            String orderString = OrderUtil.getOrderInfo(order);
            if (!result.Contains(orderString))
            {
                return Json(new { code = 201, desc = "数据验证不通过" });
            }

            //验证数据的签名,以[&sign_type="RSA"&sign=]为界,前面的为(原始数据&支付结果),后面的为带双引号的签名结果,在验证签名时,记录把开头和结尾的引号trim掉
            String[] a = result.Split(new String[] { "&sign_type=\"RSA\"&sign=" }, StringSplitOptions.None);
            bool isPass = RSAFromPkcs8.verify(a[0], a[1].Trim(new char[] { '"' }), Config.Public_key, Config.Input_charset);
            if (!isPass)
            {
                return Json(new { code = 201, desc = "数据签名不相符" });
            }

            //验证是否包含""这样的支付结果
            if (!a[0].Contains("&success=\"true\""))
            {
                return Json(new { code = 201, desc = "支付失败" });
            }

            order.TradeNo = "";
            order.TradeStatus = "COMPLETED";//只说明是同步验证成功,应该尽量依靠服务器异步验证

            if (order.IsBalance != 1)
            {
                user.NimUserEx.Coins = order.Coin + (user.NimUserEx.Coins ?? 0);
                order.IsBalance = 1;
            }

            entities.SaveChanges();
            return Json(new { code = 200, desc = "支付成功", info = new { user.Username, Nickname = user.NimUserEx.Name, user.NimUserEx.Coins } });
        }

        [HttpPost]
        public ActionResult VerifyPayPal(String orderId, String paymentId)
        {
            Orders order = entities.Orders.Find(orderId);
            if (order == null)
            {
                return Json(new { code = 201, desc = "指定订单不存在" });
            }

            NimUser user = entities.NimUser.SingleOrDefault(o => o.Username == order.UserName);



            //OAuthTokenCredential tokenCredential = new OAuthTokenCredential("<CLIENT_ID>", "<CLIENT_SECRET>");
            //string accessToken = tokenCredential.GetAccessToken();

            // ### Api Context
            // Pass in a `APIContext` object to authenticate 
            // the call and to send a unique request id 
            // (that ensures idempotency). The SDK generates
            // a request id if you do not pass one explicitly. 
            // See [Configuration.cs] to know more about APIContext.
            var apiContext = Configuration.GetAPIContext();
            Payment payment = Payment.Get(apiContext, paymentId);

            //You should verify that the Payment:
            //Is approved ("state": "approved").
            //Contains a Transaction with:
            //An Amount with total and currency values that match your expectation.
            //A Sale that is completed (in related_resources, with "state": "completed").

            if (payment.state != "approved")
            {
                return Json(new { code = 201, desc = "交易记录验证不成功" });
            }

            //由手机端传过来的信息,如支付总额,货币
            String clientAmount = order.Amount + "";
            String clientCurrency = order.Currency + "";


            Transaction d = payment.transactions[0];

            String serverAmount = d.amount.total;
            String serverCurrentcy = d.amount.currency;
            String saleState = d.related_resources[0].sale.state;

            //插入数据库

            //验证交易总额
            if (clientAmount != serverAmount)
            {
                return Json(new { code = 201, desc = "交易总额验证不成功" });
            }

            //验证货币类型
            if (clientCurrency != serverCurrentcy)
            {
                return Json(new { code = 201, desc = "货币类型验证不成功" });
            }

            //验证交易状态
            if (saleState != "completed")
            {
                return Json(new { code = 201, desc = "交易状态验证不成功" });
            }

            //保存数据
            order.TradeNo = paymentId;
            order.TradeStatus = "completed";

            //平衡学币  
            user.NimUserEx.Coins = order.Coin + (user.NimUserEx.Coins ?? 0);
            order.IsBalance = 1;
            entities.SaveChanges();

            return Json(new { code = 200, desc = "支付成功", info = new { user.Username, user.NimUserEx.Name, user.NimUserEx.Coins } });
        }

        [HttpPost]
        public ActionResult OrderRecords(String username, Int32 skip, Int32 take)
        {
            var temp = entities.Orders.Where(o => o.UserName == username).OrderByDescending(o => o.CreateTime).Skip(skip).Take(take).ToList();
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Currency,
                    o.Amount,
                    o.Main,
                    o.Body,
                    o.Coin,
                    CreateTime = (o.CreateTime == null ? "" : o.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")),
                    TradeStatus = (o.TradeStatus == "TRADE_SUCCESS" || o.TradeStatus == "TRADE_FINISHED" || o.TradeStatus == "completed" || o.TradeStatus == "COMPLETED") ? "SUCCESS" : "FAILURE"
                })
            });
        }

        /// <summary>
        /// 手动通过Paypal订单Id查询订单记录
        /// </summary>
        /// <param name="paymentId">Paypal订单Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PaypalManual(String paymentId)
        {
            var apiContext = Configuration.GetAPIContext();
            Payment payment = Payment.Get(apiContext, paymentId);
            return Json(payment);
        }

        public ActionResult AlipayManual(String d)
        {

            return Json(null);
        }

        /// <summary>
        /// 支付宝异步通知
        /// 详细情况请看 https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.viFEzs&treeId=59&articleId=103666&docType=1
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AlipayNotify()
        {
            SortedDictionary<string, string> sPara = new SortedDictionary<string, string>();

            var form = Request.Form;
            var keys = Request.Form.AllKeys;
            foreach (var key in keys)
            {
                sPara.Add(key, form[key]);
            }

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, form["notify_id"], form["sign"]);

                //打印日志
                logger.Debug(String.Format("VerifyResult:{0}  sPara:{1}", verifyResult, String.Join(",", sPara.Select(o => String.Format("[{0},{1}]", o.Key, o.Value)))));

                if (verifyResult)//验证成功
                {
                    //商户订单号                
                    string out_trade_no = form["out_trade_no"];
                    //支付宝交易号                
                    string trade_no = form["trade_no"];
                    //交易状态
                    string trade_status = form["trade_status"];


                    Orders order = entities.Orders.Find(out_trade_no);
                    if (order == null)
                    {
                        String msg = String.Format("{0} Order does not exist", out_trade_no);
                        logger.Debug(msg);
                        return Content(msg);//如果没有查询到订单,则返回失败
                    }

                    //业务数据处理注意事项
                    //商户需要验证该通知数据中的out_trade_no是否为商户系统中创建的订单号，并判断total_fee是否确实为该订单的实际金额（即商户订单创建时的金额），
                    //同时需要校验通知中的seller_id（或者seller_email) 是否为out_trade_no这笔单据的对应的操作方（有的时候，一个商户可能有多个seller_id / seller_email），
                    //上述有任何一个验证不通过，则表明本次通知是异常通知，务必忽略。在上述验证通过后商户必须根据支付宝不同类型的业务通知，正确的进行不同的业务处理，并且过滤重复的通知结果数据。
                    //在支付宝的业务通知中，只有交易通知状态为TRADE_SUCCESS或TRADE_FINISHED时，支付宝才会认定为买家付款成功。 
                    //如果商户需要对同步返回的数据做验签，必须通过服务端的签名验签代码逻辑来实现。如果商户未正确处理业务通知，存在潜在的风险，商户自行承担因此而产生的所有损失。

                    //返回状态
                    //程序执行完后必须打印输出“success”（不包含引号）。如果商户反馈给支付宝的字符不是success这7个字符，支付宝服务器会不断重发通知，直到超过24小时22分钟。
                    //一般情况下，25小时以内完成8次通知（通知的间隔频率一般是：4m,10m,10m,1h,2h,6h,15h）；
                    //支付宝异步通知的三种状态是分别通知的,
                    //如当WAIT_BUYER_PAY时会发送异步通知,没有收到返回success
                    //这时TRADE_SUCCESS异步通知来了,我们正确返回了success
                    //4分钟之后,支付宝会把WAIT_BUYER_PAY异步通知再发送一次,但是TRADE_SUCCESS由于上一次的TRADE_SUCCESS我们正确返回了数据,所以不会再来通知我们了
                    //同时,如果在第4分钟时,我们收到了WAIT_BUYER_PAY异步通知,但是我们没有正确返回success,那么再过10分钟,关于这个状态的异步通知还是会推送过来,直到我们正确返回success或者时间到期为止
                    if (trade_status == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的

                        order.TradeNo = trade_no;
                        order.TradeStatus = trade_status;
                        entities.SaveChanges();
                        logger.Debug("success");
                        return Content("success");
                    }
                    else if (trade_status == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序

                        //注意：
                        //付款完成后，支付宝系统发送该交易状态通知
                        //请务必判断请求时的total_fee、seller_id与通知时获取的total_fee、seller_id为一致的

                        if (order.TradeStatus == "TRADE_FINISHED")
                        {
                            logger.Debug("success");
                            return Content("success");
                        }

                        order.TradeNo = trade_no;
                        order.TradeStatus = trade_status;
                        entities.SaveChanges();
                        logger.Debug("success");
                        return Content("success");
                    }
                    else if (trade_status == "WAIT_BUYER_PAY")
                    {
                        if (order.TradeStatus == "TRADE_FINISHED" || order.TradeStatus == "TRADE_SUCCESS")
                        {
                            logger.Debug("success");
                            return Content("success");
                        }

                        order.IsBalance = 0;//不平衡学币
                        order.TradeNo = trade_no;
                        order.TradeStatus = trade_status;
                        entities.SaveChanges();
                        logger.Debug("success");
                        return Content("success");
                    }

                    logger.Debug("failure");
                    return Content("failure");
                }
                else//验证失败
                {
                    logger.Debug("verification failed");
                    return Content("verification failed");
                }
            }
            else
            {
                // Response.Write("无通知参数");
                logger.Debug("Parameters is null");
                return Content("Parameters is null");
            }
        }
    }

}
