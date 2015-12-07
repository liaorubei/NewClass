using ChineseChat.Library;
using Newtonsoft.Json;
using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class UserController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult Search()
        {
            List<Customer> customers = entities.Customer.ToList();
            return Json(new { code = 200, desc = "", info = new { accid = "", token = "", others = customers.Select(o => new { o.AccId, o.NickName, o.Email }) } });
        }

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            customer.AccId = Guid.NewGuid().ToString().Replace("-", "");

            if (!CheckUser(customer))
            {

                ChineseChat.Library.User a = new ChineseChat.Library.User();
                a.Accid = customer.AccId;
                a.Token = EncryptionUtil.Md5Encode(customer.AccId + NimUtil.AppKey);
                a.Name = customer.NickName;
                a.Icon = customer.Icon;


                //先在云信去创建帐号,如果成功,则在本地保存账号             
                Answer answer = NimUtil.UserCreate(a);

                if (200 == answer.code)
                {
                    customer.Password = EncryptionUtil.Md5Encode(customer.Password);
                    customer.CreateDate = DateTime.Now;
                    customer.Icon = answer.info.token;//暂时用这个属性来保存token用来测试
                    entities.Customer.Add(customer);
                    entities.SaveChanges();
                    return Json(answer);
                }
                else
                {
                    return Json(answer);
                }
            }
            else
            {
                return Json(new { code = 20000, desc = "帐号已经存在" });
            }
        }

        [HttpPost]
        public ActionResult Login(String username, String password)
        {
            Answer answer = new Answer();
            Customer customer = entities.Customer.FirstOrDefault(o => o.Account == username);
            if (customer != null && EncryptionUtil.VerifyMd5(password, customer.Password))
            {
                //记录在线状态
                customer.IsOnline = 1;
                entities.SaveChanges();

                //提示登录成功
                answer.code = 200;
                answer.info = new Info() { accid = customer.AccId, token = EncryptionUtil.Md5Encode(customer.AccId + NimUtil.AppKey) };
                return Json(answer);
            }
            else
            {
                answer.code = StatusCode.登录失败;
                return Json(answer);
            }
        }

        [HttpPost]
        public ActionResult Logout(String accid)
        {
            Customer customer = entities.Customer.Find(accid); ;
            customer.IsOnline = 0;
            entities.SaveChanges();
            return Json(new Answer() { code = 200 });
        }

        /// <summary>
        /// 检查是否已经存在该用户名
        /// </summary>
        /// <param name="user">要检查的用户名</param>
        private bool CheckUser(Customer user)
        {
            return entities.Customer.Where(o => o.Account == user.Account).Count() > 0;
        }

        [HttpPost]
        public ActionResult Update(FormCollection collection)
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

        // GET: Api/User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/User/Delete/5
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

        public ActionResult Refresh(String accid)
        {

            Int64 now = DateTime.Now.Ticks;


            return Json("");
        }










    }
}
