using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class DocumentController : BaseController
    {

        public ActionResult Select()
        {
            return View();
        }

        public ActionResult Statistics(String keyword, Int32? pageNum, Int32? numPerPage)
        {
            System.Linq.Expressions.Expression<Func<Models.View_Document_PlayCount, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicate = o => o.Title.Contains(keyword) || o.TitleTwo.Contains(keyword) || o.TitleSubCn.Contains(keyword) || o.TitleSubEn.Contains(keyword);
            }

            var model = entities.View_Document_PlayCount.Where(predicate).OrderByDescending(o => o.PlayCount).ThenByDescending(o => o.AddDate).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewBag.Keyword = keyword;
            ViewData.Model = model;
            return View();
        }

        public ActionResult PlayCount(Int32 id, Int32? pageNum, Int32? numPerPage)
        {
            ViewData.Model = entities.Playlist.Where(o => o.DocumentId == id).OrderByDescending(o => o.PlayAt).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewBag.Id = id;
            return View();
        }
    }
}