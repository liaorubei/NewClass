using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class LevelController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult Select()
        {
            var temp = entities.Level.Where(o => o.Show == 1).OrderBy(o => o.Sort);
            return Json(new { code = 200, desc = "查询成功", info = temp.Select(o => new { o.Id, o.Name, o.Sort, DocsCount = o.Document.Count }) });
        }

        [HttpPost]
        public ActionResult SelectLevelAndFolders()
        {
            var temp = entities.Level.Where(o => o.Show == 1).OrderBy(o => o.Sort).ToList();
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.Sort,
                    Folders = o.Folder.OrderByDescending(f => f.Id).Take(25).Select(f => new { f.Id, f.Name, DocsCount = f.Document.Count(d => d.AuditCase == AuditCase.审核) })
                })
            });
        }

    }
}
