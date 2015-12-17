using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class GroupController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult Select(String accid)
        {
            Expression<Func<Group, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(accid))
            {
                predicate = o => o.Host == accid;
            }

            List<Group> groups = entities.Group.Where(predicate).ToList();

            return Json(new { code = 200, desc = "查询成功", info = groups.Select(o => new { o.Id, o.Host, o.Name, o.Time, o.Theme }) });
        }


        [HttpPost]
        public ActionResult Create(Group group)
        {
            group.Id = Guid.NewGuid().ToString().Replace("-", "");
            group.CreateDate = DateTime.Now;


            entities.Group.Add(group);
            entities.SaveChanges();


            return Json(new { code = 200, desc = "创建成功", info = new { group.Id, group.Name, group.Host } });
        }

        [HttpPost]
        public ActionResult Remove(String id)
        {
            Group group = entities.Group.Find(id);
            entities.Group.Remove(group);
            entities.SaveChanges();
            return Json(new { code = 200, desc = "解散成功" });
        }

        [HttpPost]
        public ActionResult Accede(String id, String accid)
        {

            return Json(new { code = 200, desc = "加入成功" });

        }

        [HttpPost]
        public ActionResult Modify(Group group)
        {
            Group model = entities.Group.Find(group.Id);
            model.Name = group.Name;
            model.Theme = group.Theme;
            model.Notice = group.Notice;
            model.Time = group.Time;
            entities.SaveChanges();

            return Json(new { code = 200, desc = "修改成功" });
        }

    }
}
