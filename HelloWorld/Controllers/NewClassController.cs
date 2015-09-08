using StudyOnline.Models;
using StudyOnline.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Controllers
{
    public class NewClassController : Controller
    {
        private StudyOnlineEntities db = new StudyOnlineEntities();

        public ActionResult Index()
        {
            return Content("dsfsadfsdf");
        }

        public ActionResult Docs()
        {
            var data = db.Document.Where(t => t.Id > 0).OrderByDescending(t => t.AddDate).ToPagedList(1, 10);

            var d = from o in data select new { o.Id, o.Title, o.SoundPath };
            var m = data.Select(t => new { id = t.Id, title = t.Title, soundPath = t.SoundPath, levelId = t.LevelId });
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DocsByLevel(int levelId)
        {
            var temp = db.Document.Where(t => t.LevelId == levelId).OrderByDescending(t => t.AddDate).ToList();
            var data = temp.Select(t => new { t.Id, t.Title, t.SoundPath, t.Duration, t.Length });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DocsByLevelId(int id)
        {
            var temp = db.Document.Where(t => t.LevelId == id).OrderByDescending(t => t.AddDate).ToList();
            var data = temp.Select(t => new { t.Id, t.Title, t.SoundPath, t.Duration, t.Length });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Levels()
        {
            var data = db.Level.ToList();
            var m = data.Select(t => new { t.Id, t.LevelName, DocCount = t.Document.Count });
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DocById(int id)
        {
            var temp = db.Document.Find(id);
            Lyric lyric = LyricParser.Parse(temp.Lyrics);
            List<Line> lines = new List<Line>();
            Regex regex = new Regex(@"\<[\s\S]*\>");
            foreach (var item in lyric.Lines)
            {
                Line line = new Line();
                line.TimeLabel = item.Key;
                line.Original = item.Value.IndexOf('<') > 0 ? item.Value.Substring(0, item.Value.IndexOf('<')) : item.Value;
                line.Translate = regex.Match(item.Value).Value.Replace("<", "").Replace(">", "");
                lines.Add(line);
            }

            var data = new { temp.Id, temp.Title, Lyrics = lines.Select(t => new { TimeLabel = t.TimeLabel.TotalMilliseconds, t.Original, t.Translate }), temp.SoundPath };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLatestPackage()
        {
            var temp = db.UploadFile.Where(t => t.Extension == ".apk").OrderByDescending(t => t.AddDate).First();
            var data = new { PackageSize = temp.Size, PackagePath = temp.Path, UpgradeInfo = temp.Info, VersionName = temp.VersionName };
            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}
