using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class AuditController : Controller
    {
        private StudyOnline.Models.StudyOnlineEntities entities = new Models.StudyOnlineEntities();
        public ActionResult Select(String keyword, Int32? o, Int32? r)
        {

            int pageIndex = 1;
            int pageSize = 25;
            ViewData.Model = entities.Teacherreginfo.OrderByDescending(i=>i.CreateDate).ToPagedList(pageIndex, pageSize);
            return View();
        }
    }
}