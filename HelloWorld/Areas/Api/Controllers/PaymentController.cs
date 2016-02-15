
using PayPal.Api;
using PayPal.Sample;
using System;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class PaymentController : Controller
    {
        [HttpPost]
        public ActionResult Verify(String paymentId, String clientPaymentJson)
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

            if (payment.state != "approved")
            {
                return Json(new { code = 201, desc = "交易记录验证不成功", info = payment });
            }

            //由手机端传过来的信息,如支付总额,货币
            String clientAmount = "";
            String clientCurrency = "";


            Transaction d = payment.transactions[0];

            String serverAmount = d.amount.total;
            String serverCurrentcy = d.amount.currency;
            String saleState = d.related_resources[0].sale.state;

            //插入数据库

            //验证交易总额
            if (clientAmount != serverAmount)
            {

            }

            //验证货币类型
            if (clientCurrency != serverCurrentcy)
            {

            }

            //验证交易状态
            if (saleState != "completed")
            {

            }

            //插入交易明细

            //保存数据
            return Json(payment);
        }

        [HttpPost]
        public ActionResult CreateOrder(Int32 id, Payment payment, String currency, double amount, double quantity, double price, string main, String body, string trade_no, String trade_status)
        {



            return Json("");
        }



    }

}
