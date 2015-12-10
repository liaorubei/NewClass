using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class StudentController : Controller
    {
        private StudyOnlineEntities entites = new StudyOnlineEntities();
        // 请求老师
        [HttpPost]
        public ActionResult Call(String accid)
        {
            Session["teachers"] = new List<Customer>();
            Session["students"] = new List<Customer>();

            Int64 now = DateTime.Now.Ticks - 3000000000L;
            Teacher teacher = entites.Teacher.OrderBy(o => o.EnqueueTime).FirstOrDefault(o => o.IsOnline == 1 && o.IsAvailable == 1);// && o.LastRefresh > now);
            if (teacher == null)
            {
                return Json(new { code = 2001, desc = "暂时没有老师" });
            }
            teacher.IsAvailable = 0;
            entites.SaveChanges();
            return Json(new { code = 200, accid = teacher.AccId, name = teacher.Customer.NickName });
        }

        // GET: Api/Student/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Api/Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Api/Student/Create
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

        // GET: Api/Student/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Api/Student/Edit/5
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

        // GET: Api/Student/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/Student/Delete/5
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
