using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class ThemeController : Controller
    {
        private StudyOnline.Models.StudyOnlineEntities entities = new StudyOnline.Models.StudyOnlineEntities();

        [HttpPost]
        public ActionResult Select(Int32 take, Int32 skip)
        {
            var temp = entities.Theme.OrderBy(o => o.Id).Skip(skip).Take(take).ToList();
            return Json(new { code = 200, desc = "", info = temp.Select(o => new { o.Id, o.Name }) });
        }


        public ActionResult GetById(int id)
        {
            Theme theme = entities.Theme.Find(id);
            return Json(new { code = 200, desc = "查询成功", info = new { theme.Id, theme.Name, Questions = theme.Question.Select(o => new { o.Id, o.Name }) } });
        }

        [HttpPost]
        public ActionResult HsLevelAndTheme()
        {
            var hsLevels = entities.HsLevel.OrderBy(o => o.Id);
            return Json(new { code = 200, desc = "查询成功", info = hsLevels.Select(o => new { o.Id, o.Name, Theme = o.Theme.Select(t => new { t.Id, t.Name }) }) });
        }

        // POST: Api/Theme/Create
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

        // GET: Api/Theme/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Api/Theme/Edit/5
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

        // GET: Api/Theme/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/Theme/Delete/5
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
