using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class FolderController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();
        [HttpPost]
        public ActionResult GetByLevelId(Int32 levelId, Int32 skip, Int32 take)
        {
            var temp = entities.Folder.Where(o => o.LevelId == levelId).OrderByDescending(o => o.Id).Skip(skip).Take(take);
            return Json(new { code = 200, desc = "查询成功", info = temp.Select(o => new { o.Id, o.Name, DocsCount = o.Document.Count, o.LevelId }) });
        }

    }
}
