using NAudio.Wave;
using StudyOnline.Models;
using StudyOnline.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Controllers
{
    public class AdminController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Player() { return View(); }

        public ActionResult DocsList(FormCollection form)
        {
            foreach (var item in form.AllKeys)
            {

                Console.WriteLine("key=" + item);
            }

            Func<Document, bool> predicate = d => true;

            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            PagedList<Document> docs = entities.Document.Where(predicate).OrderByDescending(d => d.AddDate).OrderByDescending(t => t.AddDate).ToPagedList(pageIndex, pageSize);
            ViewBag.Docs = docs;

            List<Level> levels = entities.Level.ToList();
            ViewBag.Levels = levels;

            return View();
        }

        public ActionResult DocsCreate(int? id)
        {
            if (id != null)
            {
                ViewData.Model = entities.Document.FirstOrDefault(d => d.Id == id);
            }
            else
            {
                ViewData.Model = new Document() { Id = 0 };
            }
            List<Level> levels = entities.Level.OrderBy(l => l.Id).ToList();
            ViewBag.Levels = levels;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DocsCreate(Document doc)
        {
            if (doc.Id > 0)
            {
                Document oldDoc = entities.Document.FirstOrDefault(d => d.Id == doc.Id);
                oldDoc.LevelId = doc.LevelId;
                oldDoc.Title = doc.Title;
                oldDoc.Lyrics = doc.Lyrics;
                oldDoc.Contents = doc.Contents;
                oldDoc.SoundPath = doc.SoundPath;
                oldDoc.AddDate = DateTime.Now;
            }
            else
            {
                doc.AddDate = DateTime.Now;
                entities.Document.Add(doc);
            }
            entities.SaveChanges();

            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminDocsList", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        public ActionResult DocsDetail(int id) { return View(); }
        [HttpPost]
        public ActionResult DocsDelete(int id)
        {
            var doc = entities.Document.FirstOrDefault(d => d.Id == id);
            entities.Document.Remove(doc);
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminDocsList", rel = "", callbackType = "", forwardUrl = "" };
            return Json(data);
        }

        public ActionResult LevelList(FormCollection form)
        {
            PagedList<Level> levels = entities.Level.OrderBy(l => l.Id).ToPagedList(1, 20);
            ViewBag.Levels = levels;
            return View();
        }

        public ActionResult LevelCreate(int? id)
        {
            if (id != null)
            {
                ViewData.Model = entities.Level.FirstOrDefault(l => l.Id == id);
            }
            else
            {
                ViewData.Model = new Level { Id = 0 };
            }
            return View();
        }

        [HttpPost]
        public ActionResult LevelCreate(Level level)
        {
            if (level.Id > 0)
            {
                Level oldLevel = entities.Level.FirstOrDefault(l => l.Id == level.Id);
                oldLevel.LevelName = level.LevelName;
            }
            else
            {
                entities.Level.Add(level);
            }
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminLevelList", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        public ActionResult LevelDelete(int id)
        {
            Level level = entities.Level.FirstOrDefault(l => l.Id == id);
            entities.Level.Remove(level);
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminLevelList", rel = "", callbackType = "", forwardUrl = "" };
            return Json(data);
        }

        [HttpPost]
        public ActionResult Uploadify(HttpPostedFileBase file)
        {
            String fileName = "";
            String filePath = String.Format("/{0}/{1}/{2}{3}", "File", DateTime.Now.ToString("yyyyMMdd"), Guid.NewGuid(), Path.GetExtension(file.FileName));
            FileInfo info = new FileInfo(Server.MapPath("~" + filePath));
            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }
            fileName = file.FileName;

            file.SaveAs(info.FullName);
            var data = new { fileName = fileName, filePath = filePath };
            return Json(data);
        }

        [HttpPost]
        public ActionResult UploadifyAudio(HttpPostedFileBase file)
        {
            String fileName = "";
            String filePath = String.Format("/{0}/{1}/{2}{3}", "File", DateTime.Now.ToString("yyyyMMdd"), Guid.NewGuid(), Path.GetExtension(file.FileName));
            FileInfo info = new FileInfo(Server.MapPath("~" + filePath));
            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }

            Mp3FileReader reader = new Mp3FileReader(file.InputStream);
            double Duration = reader.TotalTime.TotalMilliseconds;

            fileName = file.FileName;
            file.SaveAs(info.FullName);
            var data = new { fileName = fileName, filePath = filePath, Duration, Length = file.ContentLength };
            return Json(data);
        }

        [HttpPost]
        public ActionResult UploadifyFull(HttpPostedFileBase file)
        {
            String fileName = "";
            String filePath = String.Format("/{0}/{1}/{2}{3}", "File", DateTime.Now.ToString("yyyyMMdd"), Guid.NewGuid(), Path.GetExtension(file.FileName));
            FileInfo info = new FileInfo(Server.MapPath("~" + filePath));
            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }
            fileName = file.FileName;

            file.SaveAs(info.FullName);
            var data = new { fileName = fileName, filePath = filePath, Size = file.ContentLength, Extension = Path.GetExtension(file.FileName) };
            return Json(data);
        }

        public ActionResult ApkIndex()
        {
            PagedList<UploadFile> files = entities.UploadFile.Where(t => t.Extension == ".apk").OrderByDescending(t => t.AddDate).ToPagedList(1, 20);
            ViewBag.UploadFiles = files;
            return View();
        }

        public ActionResult ApkCreate(int? id)
        {
            if (id != null)
            {
                ViewData.Model = entities.UploadFile.Find(id);
            }
            else
            {
                ViewData.Model = new UploadFile() { Id = 0 };
            }
            return View();
        }
        [HttpPost]
        public ActionResult ApkCreate(UploadFile upload)
        {
            if (upload.Id > 0)
            {
                UploadFile context = entities.UploadFile.Find(upload.Id);
                context.Info = upload.Info;
                context.Path = upload.Path;
                context.Size = upload.Size;
                context.Extension = upload.Extension;
                context.VersionName = upload.VersionName;
            }
            else
            {
                upload.AddDate = DateTime.Now;
                entities.UploadFile.Add(upload);
            }
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminApkIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        public ActionResult ApkDelete(int id)
        {
            UploadFile file = entities.UploadFile.Find(id);
            entities.UploadFile.Remove(file);
            entities.SaveChanges();

            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminApkIndex", rel = "", callbackType = "", forwardUrl = "" };
            return Json(data);
        }
    }
}
