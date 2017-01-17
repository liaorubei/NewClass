using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class PaymentController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();
        public ActionResult Select(DateTime? date, Int32? pageNum, Int32? numPerPage)
        {
            ViewBag.PageIndex = pageNum;
            ViewBag.PageSize = numPerPage;
            ViewData.Model = entities.NimUser.Where(o => o.Category == 1).OrderBy(o => o.Username).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            return View();
        }
    }
}