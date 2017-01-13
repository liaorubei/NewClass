using Kfstorm.LrcParser;
using log4net;
using StudyOnline.Models;
using StudyOnline.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Controllers
{
    public class HomeController : Controller
    {
        private StudyOnlineEntities db = new StudyOnlineEntities();
        private int pageSize = 15;
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index(int? index, int? level)
        {
            //取出文章等级数据,因为文章等级要一直显示(前提是要求在浏览器端显示)
            ViewBag.Levels = db.Level.Where(o => o.ShowBrowser == 1).ToList();
            Expression<Func<View_Document, bool>> whereLevelId = o => true;
            if (level != null)
            {
                whereLevelId = o => o.LevelId == level;
            }
            ViewData.Model = db.View_Document.Where(o => o.ShowBrowser == 1 && o.AuditCase == AuditCase.审核).OrderByDescending(o => o.AuditDate).Where(whereLevelId).ToPagedList(index ?? 0, pageSize);
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

        public ActionResult InitAudioDuration()
        {
            var docs = db.Document.Where(t => t.Duration == null);
            List<Double> durs = new List<double>();
            foreach (var item in docs)
            {
                FileInfo info = new FileInfo(Server.MapPath("~" + item.SoundPath));
                if (info.Exists)
                {
                    //FileStream stream = info.Open(FileMode.Open);
                    //Mp3Frame frame = Mp3Frame.LoadFromStream(stream);

                    //WaveFormat sourceFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2, frame.FrameLength, frame.BitRate);
                    //Mp3FileReader reader = new Mp3FileReader(stream, builder => (new DmoMp3FrameDecompressor(sourceFormat)));

                    //item.Duration = reader.TotalTime.TotalMilliseconds;
                    //durs.Add(item.Duration ?? 0);
                    item.Length = info.Length;
                }
            }
            int result = db.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserServiceAgreement()
        {
            return View();
        }

        public ActionResult UserHelp()
        {
            return View();
        }

        public ActionResult Login()
        {
            ViewData.Model = new LoginModel() { };
            return View();
        }

        public ActionResult Signin() { return View(); }

        public ActionResult Logout()
        {
            ViewData.Model = new LoginModel() { };
            Session.Remove("CurrentUser");
            return View("Login");
        }

        public ActionResult AndroidClient()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            log.Info(String.Format("login with username={0},password={1}", model.UserName, model.Password));
            if (String.IsNullOrEmpty(model.UserName) || String.IsNullOrEmpty(model.Password))
            {
                ViewData.Model = model;
                return View();
            }

            if (model.UserName == "admin" && model.Password == "1357902468")
            {
                var superUser = new X_User() { Username = model.UserName };
                X_Role superRole = new X_Role();
                db.X_Menu.ToList().ForEach(o => superRole.X_Menu.Add(o));
                superUser.X_Role.Add(superRole);
                Session["CurrentUser"] = superUser;
                return Redirect("/Admin/Index");
            }

            if (db.X_User.Any(o => o.Username == model.UserName && o.Password == model.Password && o.IsActive == 1))
            {
                Session["CurrentUser"] = db.X_User.FirstOrDefault(o => o.Username == model.UserName && o.Password == model.Password);
                return Redirect("/Admin/Index");
            }

            return View(model);
        }

        public ActionResult LoginDialog()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginDialog(LoginModel model)
        {
            if (model.UserName == "admin" && model.Password == "1357902468")
            {
                var superUser = new X_User() { Username = model.UserName };
                X_Role superRole = new X_Role();
                db.X_Menu.ToList().ForEach(o => superRole.X_Menu.Add(o));
                superUser.X_Role.Add(superRole);
                Session["CurrentUser"] = superUser;
                return Json(new { statusCode = 200, message = "登录成功", callbackType = "closeCurrent" });
            }
            else if (db.X_User.Any(o => o.Username == model.UserName && o.Password == model.Password && o.IsActive == 1))
            {
                Session["CurrentUser"] = db.X_User.FirstOrDefault(o => o.Username == model.UserName && o.Password == model.Password);
                return Json(new { statusCode = 200, message = "登录成功", callbackType = "closeCurrent" });
            }
            else
            {
                return Json(new { statusCode = 300, message = "登录失败", callbackType = "" });
            }

        }

    }
}
