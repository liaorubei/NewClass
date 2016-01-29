using PayPal.Api;
using PayPal.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            if (clientAmount!=serverAmount)
            {

            }

            //验证货币类型
            if (clientCurrency!=serverCurrentcy)
            {

            }

            //验证交易状态
            if (saleState!= "completed")
            {

            }

            //插入交易明细

            //保存数据
            return Json(payment);
        }

        // GET: Api/Payment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Api/Payment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Api/Payment/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Api/Payment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Api/Payment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Api/Payment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/Payment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
