using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class LevelController : Controller
    {
        //
        // GET: /Api/Level/

        public ActionResult Index()
        {
            return Json(new { status=200},JsonRequestBehavior.AllowGet);
        }

    }
}
