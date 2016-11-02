using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class HskkController : BaseController
    {
        public ActionResult Select(String keyword, Int32? rank, Int32? part, Int32? pageNum, Int32? numPerPage)
        {
            System.Linq.Expressions.Expression<Func<Hskk, bool>> predicateRank = o => true;
            if (rank != null)
            {
                predicateRank = o => o.Rank == rank;
            }
            System.Linq.Expressions.Expression<Func<Hskk, bool>> predicate = o => true;
            if (part != null)
            {
                predicate = o => o.Part == part;
            }
            var temp = entities.Hskk.Where(predicateRank).Where(predicate).OrderBy(o => o.Rank).ThenBy(o => o.Part).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewData.Model = temp;
            ViewBag.Rank = rank;
            ViewBag.Part = part;
            return View();
        }

        public ActionResult Create(Int32? id)
        {
            if (id == null)
            {
                ViewData.Model = new Hskk() { Id = 0 };
            }
            else
            {
                ViewData.Model = entities.Hskk.Find(id);
            }
            return View();
        }

        public ActionResult Part(FormCollection form)
        {




            return View();
        }

        [HttpPost]
        public ActionResult Create(Hskk hskk)
        {
            if (hskk.Id > 0)
            {
                entities.Entry(hskk).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                entities.Hskk.Add(hskk);
            }
            entities.SaveChanges();
            return Json(new ResponseModel() { statusCode = "200", message = "操作成功", callbackType = "closeCurrent", navTabId = "MasterHskkSelect" });
        }

        public ActionResult Question(Int32 id)
        {
            Hskk hskk = entities.Hskk.Find(id);
            ViewData.Model = hskk;
            return View();
        }

        [HttpPost]
        public ActionResult Question(Int32 id, HskkQuestion[] questions)
        {
            Hskk hskk = entities.Hskk.Find(id);
            hskk.HskkQuestion.Clear();

            foreach (var item in questions)
            {
                hskk.HskkQuestion.Add(item);
            }
            entities.SaveChanges();
            return Json(new ResponseModel() { statusCode = "200", message = "操作成功", navTabId = "MasterHskkSelect", callbackType = "closeCurrent", rel = "" });
        }

        public ActionResult AttachmentLookup() { return View(); }

        [HttpPost]
        public ActionResult AttachmentBrightBack(HttpPostedFileBase image)
        {
            String fileName = "";
            String filePath = String.Format("/{0}/{1}/{2}{3}", "File", DateTime.Now.ToString("yyyyMMdd"), Guid.NewGuid(), Path.GetExtension(image.FileName));
            FileInfo info = new FileInfo(Server.MapPath("~" + filePath));
            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }
            fileName = image.FileName;

            image.SaveAs(info.FullName);
            var data = new { fileName = fileName, filePath = filePath, Length = image.ContentLength, Image = filePath };
            return Json(data);
        }
    }
}