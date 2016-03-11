using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class LevelController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult Select()
        {
            var temp = entities.Level.Where(o => o.Show == 1).OrderBy(o => o.Sort);
            return Json(new { code = 200, desc = "查询成功", info = temp.Select(o => new { o.Id, o.Name, o.Sort }) });
        }

        [HttpPost]
        public ActionResult SelectLevelAndFolders()
        {
            var temp = entities.Level.Where(o => o.Show == 1).OrderBy(o => o.Sort).ToList();
            return Json(new { code = 200, desc = "查询成功", info = temp.Select(o => new { o.Id, o.Name, o.Sort, Folders = o.Folder.OrderByDescending(f => f.Id).Take(25).Select(f => new { f.Id, f.Name, DocsCount = f.Document.Count }) }) });
        }

        // GET: Api/Level/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Api/Level/Create
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

        // GET: Api/Level/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Api/Level/Edit/5
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

        // GET: Api/Level/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/Level/Delete/5
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
