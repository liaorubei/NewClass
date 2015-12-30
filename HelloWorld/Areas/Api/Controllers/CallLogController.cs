using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class CallLogController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult Start(Int32 source, Int32 target)
        {
            CallLog log = new CallLog();
            log.Id = Guid.NewGuid().ToString().Replace("-", "");
            log.Source = source;
            log.Target = target;
            log.Start = DateTime.Now;


            entities.CallLog.Add(log);
            entities.SaveChanges();
            return Json(new { code = 200, desc = "", info = new { log.Id, log.Source, log.Target } });
        }


        [HttpPost]
        public ActionResult Finish(String id)
        {
            CallLog model = entities.CallLog.Find(id);
            model.Finish = DateTime.Now;
            entities.SaveChanges();
            return Json(new { code = 200, desc = "", info = new { model.Id, model.Source, model.Target, model.Start.Value.Ticks, model.Finish } });
        }

        // GET: Api/CallLog/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Api/CallLog/Edit/5
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

        // GET: Api/CallLog/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Api/CallLog/Delete/5
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
