using StudyOnline.Models;
using StudyOnline.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace StudyOnline.Controllers
{
    /// <summary>
    /// JSON接口类,所有的列表操作,如果相关Id为空或者小于0,都表示条件为(o=>true)
    /// </summary>
    public class NewClassController : Controller
    {
        private StudyOnlineEntities db = new StudyOnlineEntities();

        [HttpPost]
        public ActionResult Index(bool? folder)
        {
            bool b = folder ?? false;
            List<Level> levels = db.Level.ToList();
            Level l = levels.OrderBy(o => o.Sort).FirstOrDefault();
            var temp = new { code = 200, desc = "", info = levels.Select(o => new { o.Id,o.Name, o.Sort, o.Show, Folders = ((b && o.Id == l.Id) ? o.Folder.Select(f => new { f.Id, f.Name, f.Document.Count }) : null) }) };
            return Json(temp);
        }

        public ActionResult DocsByLevel(int levelId)
        {
            var temp = db.Document.Where(t => t.LevelId == levelId).OrderByDescending(t => t.AddDate).ToList();
            var data = temp.Select(t => new { t.Id, t.Title, t.TitleTwo, t.SoundPath, t.Duration, t.Length, t.LengthString });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DocsByLevelId(int id, int? skip, int? take)
        {
            var temp = db.Document.Where(t => t.LevelId == id).OrderByDescending(t => t.AddDate).Skip(skip ?? 0).Take(take ?? 20).ToList();
            var data = temp.Select(t => new { t.Id, t.Title, t.TitleTwo, t.SoundPath, t.Duration, t.Length, t.LengthString, DateString = (t.AddDate == null ? "" : t.AddDate.Value.ToString("yyyy-MM-dd")) });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocs(int? folderId, int? skip, int? take)
        {
            Func<Document, bool> predicateFolder = o => true;

            if (folderId != null && folderId > 0)
            {
                predicateFolder = o => o.FolderId == folderId;
            }

            var temp = db.Document.Where(o => o.AuditCase == AuditCase.审核).Where(predicateFolder).OrderByDescending(t => t.AuditDate).Skip(skip ?? 0).Take(take ?? 20).ToList();
            var data = temp.Select(t => new
            {
                t.Id,
                t.Title,
                TitleOne = t.Title,
                t.TitleTwo,
                t.SoundPath,
                t.Duration,
                t.Length,
                t.LengthString,
                DateString = (t.AuditDate == null ? "" : t.AuditDate.Value.ToString("yyyy-MM-dd")),
                Date = (t.AuditDate == null ? "" : t.AuditDate.Value.ToString("yyyy-MM-dd")),
                Size = t.Length,
                Time = t.Duration
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Levels()
        {
            var data = db.Level.ToList();
            var m = data.Select(t => new { t.Id, t.Name, DocCount = t.Document.Count, t.Sort, t.Show });
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得指定levelId下的文件夹,如果levelId为空,则获取所有的文件夹
        /// </summary>
        /// <param name="levelId">指定等级的Id</param>
        /// <returns></returns>
        public ActionResult Folders(int? levelId)
        {
            Func<Folder, bool> predicate = o => true;
            if (levelId != null && levelId > 0)
            {
                predicate = o => o.LevelId == levelId;
            }

            var data = db.Folder.Where(predicate).OrderByDescending(o => o.Id);
            var m = data.Select(o => new
            {
                o.Id,
                o.Name,
                o.LevelId,
                DocsCount = o.Document.Count
            });
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

            var data = new
            {
                temp.Id,
                temp.Title,
                TitleOne = temp.Title,
                temp.TitleTwo,
                Lyrics = lines.Select(t => new { TimeLabel = t.TimeLabel.TotalMilliseconds, t.Original, t.Translate }),
                temp.SoundPath,
                temp.Length,
                temp.Duration,
                temp.LengthString,
                temp.AddDate,
                Date = (temp.AuditDate == null ? "" : temp.AddDate.Value.ToString("yyyy-MM-dd")),
                Size = temp.Length,
                Time = temp.LengthString
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLatestPackage()
        {
            var temp = db.UploadFile.Where(t => t.Extension == ".apk").OrderByDescending(t => t.AddDate).First();
            var data = new { PackageSize = temp.Size, PackagePath = temp.Path, UpgradeInfo = temp.Info, VersionName = temp.VersionName };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AndroidCheckUpdate(Int32 versionType)
        {
            Android android = db.Android.Where(o => o.VersionType == versionType).OrderByDescending(o => o.CreateDate).First();
            return Json(new { android.PackagePath, android.PackageSize, android.UpgradeInfo, android.VersionName, android.VersionType });
        }

    }
}
