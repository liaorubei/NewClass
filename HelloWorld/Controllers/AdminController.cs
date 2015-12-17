using NAudio.Wave;
using Newtonsoft.Json.Linq;
using StudyOnline.Filters;
using StudyOnline.Models;
using StudyOnline.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Controllers
{
    [MyAuthorizationFilter]
    public class AdminController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();
        public ActionResult Index()
        {
            ViewBag.Menus = new List<Menu>();
            return View();
        }

        public ActionResult Player() { return View(); }

        //文档相关
        public ActionResult DocsList(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            //检索处理
            Func<Document, bool> predicateKeyWord = d => true;
            Func<Document, bool> predicateLevelId = d => true;
            Func<Document, bool> predicateFolderId = o => true;

            String keyword = form["keyword"];
            if (!String.IsNullOrEmpty(keyword))
            {
                predicateKeyWord = o => o.Title.Contains(keyword) || o.TitleTwo.Contains(keyword);
            }

            int levelId = ConvertUtil.ToInt32(form["levelId"], -1);
            if (levelId > 0)
            {
                predicateLevelId = o => o.LevelId == levelId;
            }

            int folderId = ConvertUtil.ToInt32(form["folderId"], -1);
            if (folderId > 0)
            {
                predicateFolderId = o => o.FolderId == folderId;
            }

            //数据和分页检索条件处理
            PagedList<Document> docs = entities.Document.Where(predicateKeyWord).Where(predicateLevelId).Where(predicateFolderId).OrderByDescending(d => d.AddDate).OrderByDescending(t => t.AddDate).ToPagedList(pageIndex, pageSize);
            ViewBag.Docs = docs;

            List<Level> levels = entities.Level.ToList();
            List<Folder> folders = entities.Folder.ToList();

            ViewBag.Levels = levels;
            ViewBag.Folders = folders;
            ViewBag.KeyWord = keyword;//关键字
            ViewBag.LevelId = levelId;//文章级别
            ViewBag.FolderId = folderId;//文件夹
            return View();
        }

        public ActionResult DocsLookup(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            //检索处理
            Func<Document, bool> predicateKeyWord = d => true;
            Func<Document, bool> predicateLevelId = d => true;
            String keyword = form["keyword"];
            if (!String.IsNullOrEmpty(keyword))
            {
                predicateKeyWord = o => o.Title.Contains(keyword) || o.TitleTwo.Contains(keyword) || o.Contents.Contains(keyword);
            }
            int levelId = ConvertUtil.ToInt32(form["levelId"], -1);
            if (levelId > 0)
            {
                predicateLevelId = o => o.LevelId == levelId;
            }

            //数据和分页检索条件处理
            PagedList<Document> docs = entities.Document.Where(predicateKeyWord).Where(predicateLevelId).Where(o => o.FolderId == null).OrderByDescending(d => d.AddDate).OrderByDescending(t => t.AddDate).ToPagedList(pageIndex, pageSize);
            ViewBag.Docs = docs;

            List<Level> levels = entities.Level.ToList();

            ViewBag.Levels = levels;
            ViewBag.KeyWord = keyword;//关键字
            ViewBag.LevelId = levelId;//文章级别
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
            List<Folder> folders = entities.Folder.OrderBy(o => o.Id).ToList();

            ViewBag.Levels = levels;
            ViewBag.Folders = folders;

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
                oldDoc.FolderId = doc.FolderId;
                oldDoc.Title = doc.Title;
                oldDoc.Lyrics = doc.Lyrics;
                oldDoc.SoundPath = doc.SoundPath;//音频路径
                oldDoc.Length = doc.Length;//文件长度
                oldDoc.TitleTwo = doc.TitleTwo;//文件的标题（翻译）
                oldDoc.LengthString = doc.LengthString;//音频文件的播放长度
                oldDoc.AuditCase = doc.AuditCase;//审核情况
                oldDoc.AuditDate = doc.AuditDate;//审核日期
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

        public ActionResult AuditDocs(Int32? id)
        {
            Document doc = entities.Document.Find(id);
            doc.AuditCase = AuditCase.审核;
            doc.AuditDate = DateTime.Now;
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminDocsList", rel = "", callbackType = "", forwardUrl = "" };
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

        //分类相关
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
                oldLevel.Sort = level.Sort;
                oldLevel.Show = level.Show;
                oldLevel.ShowBrowser = level.ShowBrowser;
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

        //文件夹相关
        public ActionResult FolderIndex()
        {
            PagedList<Folder> folders = entities.Folder.OrderByDescending(t => t.Id).ToPagedList(1, 20);
            ViewBag.Folders = folders;
            return View();
        }

        public ActionResult FolderCreate(int? id, FormCollection form)
        {
            List<Level> levels = entities.Level.ToList();
            ViewBag.Levels = levels;


            if (id == null)
            {
                ViewData.Model = new Folder() { Id = 0 };
            }
            else
            {
                ViewData.Model = entities.Folder.Find(id);
            }

            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);
            PagedList<Document> docs = entities.Document.OrderByDescending(o => o.AddDate).Where(o => true).ToPagedList(pageIndex, pageSize);
            ViewBag.Documents = docs;






            return View();
        }

        [HttpPost]
        public ActionResult FolderCreate(Folder folder, String DocsIds, FormCollection form)
        {
            if (folder.Id > 0)
            {
                Folder contextEntity = entities.Folder.Find(folder.Id);
                contextEntity.Name = folder.Name;
                contextEntity.LevelId = folder.LevelId;
            }
            else
            {
                entities.Folder.Add(folder);
            }
            entities.SaveChanges();

            // context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
            // context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));

            //首先清除原来的数据
            //  entities.Database.ExecuteSqlCommand("update document set folderId=null where folderId=@folderId", new SqlParameter("@folderId", folder.Id));

            //再保存新的数据
            //if (!String.IsNullOrEmpty(form["document.DocsIds"]))
            //{
            //    entities.Database.ExecuteSqlCommand("update document set folderId=@folderId where id in(" + form["document.DocsIds"] + ")", new SqlParameter("@folderId", folder.Id));
            //}

            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminFolderIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        public ActionResult FolderCreateRight(FormCollection form)
        {

            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);


            System.Linq.Expressions.Expression<Func<Document, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(form["keyword"]))
            {
                String keyword = form["keyword"];
                predicate = o => o.Title.Contains(keyword);
                ViewBag.Keyword = form["keyword"];
            }


            PagedList<Document> docs = entities.Document.OrderByDescending(o => o.AddDate).Where(predicate).ToPagedList(pageIndex, pageSize);
            ViewBag.Documents = docs;
            return View();

        }

        public ActionResult FolderDelete(int? id)
        {
            Folder folder = entities.Folder.Find(id);
            entities.Folder.Remove(folder);
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminFolderIndex", rel = "", callbackType = "", forwardUrl = "" };
            return Json(data);
        }


        public ActionResult FolderSelect(int id)
        {
            ViewData.Model = entities.Folder.Find(id);
            return View();
        }

        [HttpPost]
        public ActionResult FolderSelect(int Id, List<int> ids)
        {
            entities.Database.ExecuteSqlCommand("UPDATE Document SET FolderId=null WHERE FolderId=@folderId", new SqlParameter("@folderId", Id));
            if (ids != null)
            {
                String sqlIn = String.Join(",", ids);
                entities.Database.ExecuteSqlCommand("UPDATE Document SET FolderId=@folderId WHERE Id in(" + sqlIn + ")", new SqlParameter("@folderId", Id));
            }
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminFolderIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }


        #region 客户管理
        public ActionResult CustomerIndex()
        {
            PagedList<Customer> customers = entities.Customer.OrderByDescending(o => o.CreateDate).ToPagedList(1, 25);
            ViewBag.Customers = customers;
            return View();
        }

        public ActionResult CustomerCreate()
        {
            PagedList<Customer> customers = entities.Customer.OrderByDescending(o => o.CreateDate).ToPagedList(1, 25);
            ViewBag.Customers = customers;
            return View();
        }

        /// <summary>
        /// 修改用户,只修改基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerUpdate(String accid)
        {
            Customer customer = entities.Customer.Find(accid);
            ViewData.Model = customer;
            return View();
        }

        [HttpPost]
        public ActionResult CustomerUpdate(Customer customer)
        {
            Customer model = entities.Customer.Find(customer.AccId);
            model.Account = customer.Account;
            model.Password = ChineseChat.Library.EncryptionUtil.Md5Encode(customer.Password);//密码加密
            model.NickName = customer.NickName;
            model.Phone = customer.Phone;
            model.Email = customer.Email;

            ChineseChat.Library.User user = new ChineseChat.Library.User();
            user.Accid = model.AccId;
            user.Name = model.NickName;
            user.Token = ChineseChat.Library.EncryptionUtil.Md5Encode(model.AccId + ChineseChat.Library.NimUtil.AppKey);//更换token

            String json = ChineseChat.Library.NimUtil.UserUpdate(user);
            JObject rss = JObject.Parse(json);
            if ("200" == rss.GetValue("code").ToString())
            {
                entities.SaveChanges();
            }

            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminCustomerIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }




        #endregion









        #region 系统管理
        public ActionResult MenuIndex()
        {
            ViewBag.Menus = new List<Menu>();
            return View();
        }

        public ActionResult MenuLookup()
        {
            return View();





        }

        public ActionResult MenuCreate(int? id)
        {
            if (id == null)
            {
                ViewData.Model = new Menu() { Id = 0 };
            }
            else
            {
                ViewData.Model = new Menu() { };
            }
            return View();
        }
        [HttpPost]
        public ActionResult MenuCreate(Menu menu)
        {
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminMenuIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }
        #endregion



        #region 用户管理

        public ActionResult UserIndex()
        {
            PagedList<User> users = entities.User.OrderBy(o => o.CreateDate).ToPagedList(1, 25);
            ViewBag.Users = users;
            return View();
        }

        [HttpGet]
        public ActionResult UserCreate(int? id)
        {
            if (id != null)
            {
                ViewData.Model = entities.User.Find(id);
            }
            else
            {
                ViewData.Model = new User() { Id = 0 };
            }
            return View();
        }

        [HttpPost]
        public ActionResult UserCreate(Models.User user)
        {
            if (user.Id > 0)
            {
                Models.User u = entities.User.Find(user.Id);
                u.UserName = user.UserName;
                u.Password = user.Password;
                u.Phone = user.Phone;
                u.Email = user.Email;
                // u.IsOnline   =user.IsOnline   ;
                //  u.CreateDate = user.CreateDate;
            }
            else
            {
                user.CreateDate = DateTime.Now;
                entities.User.Add(user);
            }
            entities.SaveChanges();

            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminUserIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }
        public ActionResult UserDelete(int? id)
        {
            User user = entities.User.Find(id);
            entities.User.Remove(user);
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminUserIndex", rel = "", callbackType = "", forwardUrl = "" };
            return Json(data);
        }


        #endregion


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
            var data = new { fileName = fileName, filePath = filePath, Length = file.ContentLength };
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





        public ActionResult Login()
        {
            HttpCookie cookie = Request.Cookies["User"];

            LoginModel model = new LoginModel();
            if (cookie != null)
            {
                model.UserName = cookie["username"];
                model.Password = cookie["password"];
                model.RememberMe = Convert.ToBoolean(cookie["RememberMe"]);
            }
            ViewData.Model = model;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {

            User user = entities.User.FirstOrDefault(o => o.UserName == model.UserName && o.Password == model.Password);
            if (user == null)
            {
                ViewData.Model = model;
                return View();
            }
            user.IsOnline = 1;
            entities.SaveChanges();

            Session["User"] = user;

            HttpCookie cookie = new HttpCookie("User");
            cookie["username"] = user.UserName;
            cookie["password"] = user.Password;
            cookie["RememberMe"] = model.RememberMe.ToString();
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");

        }
    }
}
