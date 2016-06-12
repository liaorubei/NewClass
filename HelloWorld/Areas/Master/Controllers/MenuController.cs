using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class MenuController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();

        public ActionResult Select()
        {
            var menus = entities.X_Menu.Where(o => o.ParentId == null).ToList();
            ViewData.Model = menus;
            return View();
        }

        public ActionResult Create(Int32 id)
        {
            Int32? parentId = null;
            if (id > 0)
            {
                parentId = id;
            }
            ViewData.Model = new X_Menu() { ParentId = parentId };
            return View();
        }

        [HttpPost]
        public ActionResult Create(X_Menu menu)
        {
            entities.X_Menu.Add(menu);
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "MasterMenuSelect", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        public ActionResult Update(Int32 id)
        {
            ViewData.Model = entities.X_Menu.Find(id);
            return View();
        }

        [HttpPost]
        public ActionResult Update(X_Menu menu)
        {
            X_Menu model = entities.X_Menu.Find(menu.Id);
            model.Name = menu.Name;
            model.Order = menu.Order;
            model.ParentId = menu.ParentId;
            model.Area = menu.Area;
            model.Ctrl = menu.Ctrl;
            model.Action = menu.Action;

            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "MasterMenuSelect", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        [HttpPost]
        public ActionResult Delete(Int32 id)
        {
            X_Menu model = new X_Menu() { Id = id };
            entities.Entry(model).State = System.Data.Entity.EntityState.Deleted;
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "MasterMenuSelect", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }
    }
}