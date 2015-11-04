using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebHelloWorld.Models;

namespace WebHelloWorld.Controllers
{
    public class HomeController : Controller
    {
        HelloWorldContext db = new HelloWorldContext();
        public ActionResult Index()
        {
            List<Menu> menus = db.Menu.ToList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}