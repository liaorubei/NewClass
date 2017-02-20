using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class V1Controller : Controller
    {
        [HttpGet]
        public ActionResult Hskks(Int32 id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Hskks(Int16 id)
        {
            return View();
        }
        [HttpPut]
        public ActionResult Hskks(Int64 Id)
        {
            return View();
        }
        [HttpDelete]
        public ActionResult Hskks(String id)
        {
            return View();
        }
    }
}