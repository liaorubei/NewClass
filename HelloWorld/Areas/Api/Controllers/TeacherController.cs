using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class TeacherController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult Enqueue(Int32 id)
        {
            NimUser user = entities.NimUser.Find(id);
            if (user == null)
            {
                return Json(new { code = 201, desc = "用户不存在" });
            }

            if (user.Category != 1)
            {
                return Json(new { code = 202, desc = "用户类型错误" });
            }

            user.IsOnline = 1;
            user.IsEnable = 1;
            //user.Enqueue = DateTime.Now.Ticks;
            //user.Refresh = DateTime.Now.Ticks;

            try
            {
                entities.SaveChanges();
                return Refresh(id, 0, 25);
            }
            catch (Exception ex)
            {
                return Json(new { code = 203, desc = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Dequeue(Int32 id)
        {
            NimUser user = entities.NimUser.Find(id);
            if (user == null)
            {
                return Json(new { code = 201, desc = "用户不存在" });
            }

            if (user.Category != 1)
            {
                return Json(new { code = 202, desc = "用户类型错误" });
            }
            //Queue<>

            user.IsOnline = 0;
            user.IsEnable = 0;
            //user.Enqueue = DateTime.Now.Ticks;
          //  user.Refresh = DateTime.Now.Ticks;

            try
            {
                entities.SaveChanges();
                return Refresh(id, 0, 25);
            }
            catch (Exception ex)
            {
                return Json(new { code = 203, desc = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Refresh(Int32 id, Int32 skip, Int32 take)
        {
            var all = entities.NimUser.Where(o => o.Category == 1 && o.IsOnline == 1 && o.IsEnable == 1).OrderBy(o => o.Id);

            Int32 Index = -1;

            NimUser k = all.FirstOrDefault(o => o.Id == id);
            if (k != null)
            {
                Index = all.Count();
            }

            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = new
                {
                    Index,
                    Count = all.Count(),
                    Current = k == null ? null : new { k.Id, k.Username, k.IsOnline, k.IsEnable},
                    Teacher = all.Skip(skip).Take(take).Select(o => new { o.Id, o.Username, o.IsOnline, o.IsEnable })
                }
            });
        }

    }
}
