using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebHelloWorld.Models;

namespace WebHelloWorld.Controllers
{
    public class AdminController : Controller
    {
        HelloWorldContext context = new HelloWorldContext();
        public ActionResult Index()
        {
            List<Menu> menus = context.Menu.ToList();
            ViewBag.Menus = menus;
            return View();
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
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

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
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

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
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




        public ActionResult MenuCreate()
        {
            return View();
        }

        public ActionResult MenuSelect()
        {

            List<Menu> ms = new List<Menu>();
            for (int i = 0; i < 5; i++)
            {
                Menu p = new Menu() { Name = "父目录" + i };
                p.Children = new List<Menu>();
                for (int m = 10; m < 15; m++)
                {
                    Menu c = new Menu() { Name = "子目录" + m };
                    p.Children.Add(c);
                }
                ms.Add(p);
            }

            return Json(ms, JsonRequestBehavior.AllowGet);
        }


    }
}
