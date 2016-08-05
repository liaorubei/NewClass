using StudyOnline.Filters;
using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    [MyAuthorizationFilter]
    public class ThemeController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();
        public ActionResult Select(String keyword, Int32? levelId, Int32? pageNum, Int32? numPerPage)
        {
            System.Linq.Expressions.Expression<Func<Theme, bool>> predicateKeyword = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicateKeyword = o => o.Name.Contains(keyword);
            }

            System.Linq.Expressions.Expression<Func<Theme, bool>> predicateLevelId = o => true;
            if (levelId != null)
            {
                predicateLevelId = o => o.HsLevelId == levelId;
            }

            List<SelectListItem> selectListLevelId = new List<SelectListItem>();

            ViewBag.Keyword = keyword;

            selectListLevelId.Add(new SelectListItem() { Text = "-所有的-", Value = "" });
            entities.HsLevel.ToList().ForEach(o => selectListLevelId.Add(new SelectListItem() { Text = o.Name, Value = o.Id.ToString(), Selected = o.Id == levelId, }));
            ViewBag.SelectListLevelId = selectListLevelId;

            ViewData.Model = entities.Theme.Where(o => o.IsDelete != 1).Where(predicateKeyword).Where(predicateLevelId).OrderBy(o => o.Sort).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            return View();
        }

        [HttpPost]
        public ActionResult Delete(Int32 id)
        {
            entities.Database.ExecuteSqlCommand("Update Theme set IsDelete=1 where id=@id", new SqlParameter("@id", id));
            return Json(new ResponseModel() { statusCode = "200", message = "操作成功", navTabId = "MasterThemeSelect", rel = "", callbackType = "", forwardUrl = "" });
        }
    }
}