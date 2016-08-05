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
    public class NimUserController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();
        public ActionResult Select(String keyword, Int32? category, Int32? numPerPage, Int32? pageNum)
        {

            System.Linq.Expressions.Expression<Func<View_User, bool>> predicateKeyword = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicateKeyword = o => o.Username.Contains(keyword) || o.Nickname.Contains(keyword);
            }

            System.Linq.Expressions.Expression<Func<View_User, bool>> predicate = o => true;
            if (category != null)
            {
                predicate = o => o.Category == category;
            }

            ViewData.Model = entities.View_User.Where(predicateKeyword).Where(predicate).OrderBy(o => o.CreateDate).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewBag.Keyword = keyword;
            ViewBag.Category = category;
            return View();
        }

        [HttpPost]
        public ActionResult Freeze(Int32 id)
        {
            entities.Database.ExecuteSqlCommand("UPDATE [NimUser] SET [IsActive]=0,[IsOnline]=0,[IsEnable]=0 WHERE Id=@id", new SqlParameter("@id", id));         
            return Json(new { statusCode = "200", message = "操作成功", navTabId = "MasterNimUserSelect", rel = "", callbackType = "", forwardUrl = "" });
        }
    }
}