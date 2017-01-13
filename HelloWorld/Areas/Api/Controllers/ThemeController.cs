using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class ThemeController : Controller
    {
        private StudyOnline.Models.StudyOnlineEntities entities = new StudyOnline.Models.StudyOnlineEntities();

        [HttpPost]
        public ActionResult Select(Int32? take, Int32? skip)
        {
            var temp = entities.Theme.OrderBy(o => o.Sort).Skip(skip ?? 0).Take(take ?? Int32.MaxValue).ToList();
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.NameEn,
                    o.Sort
                })
            });
        }

        [HttpPost]
        public ActionResult GetById(Int32 id)
        {
            Theme theme = entities.Theme.Find(id);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = new
                {
                    theme.Id,
                    theme.Name,
                    theme.NameEn,
                    theme.Sort,
                    Questions = theme.Question.OrderBy(o => o.Sort).Select(o => new
                    {
                        o.Id,
                        o.Name
                    })
                }
            });
        }

        [HttpPost]
        public ActionResult HsLevelAndTheme()
        {
            var hsLevels = entities.HsLevel.OrderBy(o => o.Id).ToList();
            var themes = entities.Theme.ToList();

            foreach (var item in hsLevels)
            {
                item.Theme.Clear();
            }
            hsLevels.First().Theme = themes;

            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = hsLevels.Select(o => new
                {
                    o.Id,
                    o.Name,
                    Theme = o.Theme.Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.NameEn,
                        t.Sort
                    })
                })
            });
        }

        // POST: Api/Theme/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Api/Theme/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Api/Theme/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Api/Theme/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/Theme/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
