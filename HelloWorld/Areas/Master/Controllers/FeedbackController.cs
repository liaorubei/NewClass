using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class FeedbackController : BaseController
    {

        public ActionResult Select(Int32? pageNum, Int32? numPerPage)
        {
            ViewData.Model = entities.Feedback.OrderByDescending(o => o.Createtime).ToPagedList(pageNum ?? 0, numPerPage ?? 50);
            return View();
        }

        public ActionResult Detail(Int32 id)
        {
            ViewData.Model = entities.Feedback.Find(id);
            return View();
        }
    }
}