using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class TeacherController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();

        /// <summary>
        /// 教师端刷新在线状态的方法,默认定时5分钟刷新一次
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Refresh(String accid)
        {
            Int64 now = DateTime.Now.Ticks;

            Teacher teacher = entities.Teacher.Find(accid);
            teacher.LastRefresh = now;
            entities.SaveChanges();

            long refresh = now - 3000000000L;//5分钟轮循时间
            int count = entities.Teacher.Where(o => o.IsOnline == 1 && o.IsAvailable == 1 && o.EnqueueTime < teacher.EnqueueTime && o.LastRefresh > refresh).Count();
            return Json(new { code = 200, rank = count });
        }

        /// <summary>
        /// 教师端入队方法,近时间排序,先排先得
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enqueue(String accid)
        {
         

            Int64 now = DateTime.Now.Ticks;

            Teacher teacher = entities.Teacher.Find(accid);
            teacher.IsOnline = 1;
            teacher.IsAvailable = 1;
            teacher.LastRefresh = now;
            teacher.EnqueueTime = now;
            entities.SaveChanges();

            long refresh = now - 3000000000L;//5分钟轮循时间
            Int32 rank = entities.Teacher.Where(o => o.IsOnline == 1 && o.IsAvailable == 1 && o.EnqueueTime < now && (o.LastRefresh > refresh)).Count();
            return Json(new { code = 200, rank = rank });
        }
        public ActionResult Index()
        {
            return View();
        }

        // GET: Api/Teacher/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Api/Teacher/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Api/Teacher/Create
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

        // GET: Api/Teacher/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Api/Teacher/Edit/5
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

        // GET: Api/Teacher/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/Teacher/Delete/5
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
