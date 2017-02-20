using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class ChatDataController : BaseController
    {
        // GET: Master/ChatData
        public ActionResult Select(String keyword, Int32? pageNum, Int32? numPerPage)
        {
            System.Linq.Expressions.Expression<Func<Models.CallLog, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                var ids = entities.NimUser.Where(o => o.Username.Contains(keyword) || o.NimUserEx.Name.Contains(keyword)).Select(o => o.Id).ToList();
                predicate = o => ids.Contains(o.Source) || ids.Contains(o.Target);
            }
            ViewData.Model = entities.CallLog.Where(predicate).OrderByDescending(o => o.Start).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewBag.Keyword = keyword;
            return View();
        }
    }
}