
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

namespace StudyOnline.Areas.Api.Controllers
{
    public class PaymentController : Controller
    {
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
        public ActionResult VerifyAliPay(String result, String orderId)
        {
            Orders order = entities.Orders.Find(orderId);
            if (order == null)
            {
                return Json(new { code = 201, desc = "指定订单不存在", info = order });
            }

            //如果异步通知成功,直接返回
            //  if (order.TradeStatus == "")
            // {
            //  return Json(new { code = 200, desc = "", info = order });
            // }

            //如果服务端没有收到异步通知的时候,则要验证客户端发过来的同步通知(https://doc.open.alipay.com/doc2/detail.htm?spm=0.0.0.0.bsvyrx&treeId=59&articleId=103665&docType=1)
            //1、原始数据是否跟商户请求支付的原始数据一致（必须验证这个）；
            //2、验证这个签名是否能通过。上述1、2通过后，在sign字段中success = true才是可信的。

            //构建原始数据,并验证是否一致
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
                return Json(new { code = 201, desc = "支付不成功" });
            }

            order.TradeNo = "";
            order.TradeStatus = "TRADE_FINISHED";
            entities.SaveChanges();

            return Json(new { code = 200, desc = "支付成功", info = order });
        }

        [HttpPost]
        public ActionResult VerifyPayPal(String paymentId, String orderId)
        {
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
                return Json(new { code = 201, desc = "交易记录验证不成功", info = payment });
            }

            Orders order = entities.Orders.Find(orderId);

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
                return Json(new { code = 201, desc = "交易总额验证不成功", info = payment });
            }

            //验证货币类型
            if (clientCurrency != serverCurrentcy)
            {
                return Json(new { code = 201, desc = "货币类型验证不成功", info = payment });
            }

            //验证交易状态
            if (saleState != "completed")
            {
                return Json(new { code = 201, desc = "交易状态验证不成功", info = payment });
            }

            //保存数据
            order.TradeNo = paymentId;
            order.TradeStatus = "completed";
            entities.SaveChanges();

            return Json(new { code = 200, desc = "交易成功", info = payment });
        }

        [HttpPost]
        public ActionResult OrderRecords(String username, Int32 skip, Int32 take)
        {
            var temp = entities.Orders.Where(o => o.UserName == username).Skip(skip).Take(take);
            return Json(new { code = 200, desc = "", info = temp.Select(o => new { o.Id, o.Main }) });
        }





    }

}
