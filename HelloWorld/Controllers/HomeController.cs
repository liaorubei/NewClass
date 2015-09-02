using Kfstorm.LrcParser;
using StudyOnline.Models;
using StudyOnline.Utils;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Controllers
{
    public class HomeController : Controller
    {
        private StudyOnlineEntities db = new StudyOnlineEntities();
        private int pageSize = 14;
        public ActionResult Index(int? index, int? level)
        {
            //取出文章等级数据,因为文章等级要一直显示
            ViewBag.Levels = db.Level.ToList();

            //过滤等级功能,如果没有则不过滤,如果不为空,说明要求按等级查询数据
            Expression<Func<Document, bool>> predicate = m => true;
            if (level != null)
            {
                predicate = m => m.LevelId == level;//如果不为空,则按等级要求查询数据
            }

            //分页数据要求
            //1,先排序
            //2,过滤查询参数
            //3,分页取出数据
            ViewBag.Documents = db.Document.OrderByDescending(m => m.AddDate).Where(predicate).ToPagedList(index ?? 0, pageSize);
            return View();
        }

        public ActionResult Detail(int id)
        {
            Document doc = db.Document.FirstOrDefault(m => m.Id == id);
            Lyric lyric = LyricParser.Parse(doc.Lyrics);

            ViewBag.Lrcs = lyric;
            ViewData.Model = doc;
            return View();
        }

        [HttpPost]
        public ActionResult Detail(int id, String mainbody, int Score)
        {
            var com = new Comment() { DocumentId = id, MainBody = mainbody, Score = Score };
            db.Comment.Add(com);
            db.SaveChanges();

            Document doc = db.Document.FirstOrDefault(m => m.Id == id);
            ILrcFile lrcFile = LrcFile.FromText(doc.Lyrics);

            ViewBag.Lrcs = lrcFile;
            ViewData.Model = doc;

            // return View();
            // return Content("true");
            //  return Json(json);
            return Json(new { status = 200, message = "提交成功", score = com.Score, mainbody = com.MainBody });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// 添加生词到生词本
        /// </summary>
        /// <param name="form">表单内容</param>
        /// <returns>包含status的JSON</returns>
        [HttpPost]
        public ActionResult AddVocab(FormCollection form)
        {

            return Json(new { status = 200 });
        }

        /// <summary>
        /// 头部碎片，即网站图片轮循部分
        /// </summary>
        /// <returns></returns>
        public ActionResult Header()
        {
            return PartialView();
        }

        public ActionResult GetAndroidClient()
        {
            UploadFile file = db.UploadFile.Where(t => t.Extension == ".apk").OrderByDescending(t => t.AddDate).FirstOrDefault();
            String path = Server.MapPath("~" + file.Path);
            return File(path, "application/vnd.android", "com.voc.woyaoxue.apk");
        }
    }
}
