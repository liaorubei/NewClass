using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class HskkController : Controller
    {
        private StudyOnlineEntities entites = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult GetListByRankAndPart(Int32 rank, Int32 part, Int32? skip, Int32? take)
        {
            var temp = entites.Hskk.Where(o => o.Rank == rank && o.Part == part).OrderBy(o => o.Id).Skip(skip ?? 0).Take(take ?? Int32.MaxValue).ToList();
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Rank,
                    o.Part,
                    o.Name,
                    o.Desc,
                    o.Visible,
                    o.Category
                })
            });
        }

        [HttpPost]
        public ActionResult GetById(Int32 id)
        {
            var hskk = entites.Hskk.Find(id);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = new
                {
                    hskk.Id,
                    hskk.Rank,
                    hskk.Part,
                    hskk.Name,
                    hskk.Desc,
                    hskk.Visible,
                    hskk.Category,
                    Questions = hskk.HskkQuestion.OrderBy(o => o.Sort).Select(o => new
                    {
                        o.Id,
                        o.Sort,
                        o.TextCN,
                        o.TextPY,
                        o.Image,
                        o.Audio
                    })
                }
            });
        }

        [HttpPost]
        public ActionResult GetQuestionListByHskkId(Int32 hskkId, Int32? skip, Int32? take)
        {
            var temp = entites.HskkQuestion.Where(o => o.HskkId == hskkId).OrderBy(o => o.Sort).Skip(skip ?? 0).Take(take ?? Int32.MaxValue).ToList();

            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Sort,
                    o.Image,
                    o.Audio,
                    o.TextCN,
                    o.TextPY
                })
            });
        }
    }
}