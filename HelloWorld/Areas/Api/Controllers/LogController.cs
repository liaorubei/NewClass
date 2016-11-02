using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class LogController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [HttpPost]
        public ActionResult Info(String msg)
        {
            logger.Info(msg);
            return Json(new { code = 200, desc = "记录成功" });
        }

        [HttpPost]
        public ActionResult Warn(String msg)
        {
            logger.Warn(msg);
            return Json(new { code = 200, desc = "记录成功" });
        }

        [HttpPost]
        public ActionResult Error(String msg)
        {
            logger.Error(msg);
            return Json(new { code = 200, desc = "记录成功" });
        }
    }
}