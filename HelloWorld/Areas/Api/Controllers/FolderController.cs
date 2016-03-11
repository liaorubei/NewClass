using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class FolderController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();
        [HttpPost]
        public ActionResult GetByLevelId(Int32 levelId, Int32 skip, Int32 take)
        {
            var temp = entities.Folder.Where(o => o.LevelId == levelId).OrderByDescending(o => o.Id).Skip(skip).Take(take);
            return Json(new { code = 200, desc = "查询成功", info = temp.Select(o => new { o.Id, o.Name, DocsCount = o.Document.Count, o.LevelId }) });
        }

        // GET: Api/Folder/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Api/Folder/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Api/Folder/Create
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

        // GET: Api/Folder/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Api/Folder/Edit/5
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

        // GET: Api/Folder/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/Folder/Delete/5
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
