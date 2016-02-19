using NAudio.Wave;
using Newtonsoft.Json.Linq;
using StudyOnline.Filters;
using StudyOnline.Models;
using StudyOnline.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            var doc = entities.Document.Find(id);
            entities.Document.Remove(doc);
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminDocsList", rel = "", callbackType = "", forwardUrl = "" };
            return Json(data);
        }

        #region 分类相关




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

        #endregion

        #region 文件夹相关
        public ActionResult FolderIndex(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            //关键字处理
            Expression<Func<Folder, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(form["keyword"]))
            {
                String keyword = form["keyword"];
                predicate = o => o.Name.Contains(keyword);
                ViewBag.Keyword = form["keyword"];
            }

            PagedList<Folder> folders = entities.Folder.Where(predicate).OrderByDescending(t => t.Id).ToPagedList(pageIndex, pageSize);
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

            //关键字处理
            Expression<Func<Document, bool>> predicate = o => true;
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

        #endregion

        #region 客户管理
        public ActionResult NimUserIndex(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 25);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            //关键字
            Expression<Func<NimUser, bool>> predicate = o => true;
            if (!string.IsNullOrEmpty(form["keyword"]))
            {
                String keyword = form["keyword"];
                predicate = o => o.Username.Contains(keyword) || o.NimUserEx.Name.Contains(keyword) || o.NimUserEx.Email.Contains(keyword) || o.NimUserEx.Mobile.Contains(keyword);
                ViewBag.Keyword = keyword;
            }

            Expression<Func<NimUser, bool>> predicateCategory = o => true;
            Int32 category = ConvertUtil.ToInt32(form["category"], -1);
            ViewBag.Category = category;

            if (category != -1)
            {
                predicateCategory = o => o.Category == category;
            }

            PagedList<NimUser> NimUsers = entities.NimUser.Where(predicate).Where(predicateCategory).OrderBy(o => o.CreateDate).ToPagedList(pageIndex, pageSize);
            ViewBag.NimUsers = NimUsers;
            return View();
        }


        /// <summary>
        /// 修改用户,只修改帐号信息
        /// </summary>
        /// <returns></returns>
        public ActionResult NimUserUpdate(Int32 id)
        {
            NimUser NimUser = entities.NimUser.Find(id);
            ViewData.Model = NimUser;
            return View();
        }

        [HttpPost]
        public ActionResult NimUserUpdate(NimUser nimUser)
        {
            NimUser model = entities.NimUser.Find(nimUser.Id);
            int exist = entities.NimUser.Where(o => o.Id != nimUser.Id && o.Username == nimUser.Username).Count();
            if (exist > 0)
            {
                return Json(new { statusCode = "300", message = "帐号重复" });
            }

            model.Username = nimUser.Username;
            model.Password = ChineseChat.Library.EncryptionUtil.Md5Encode(nimUser.Password);//密码加密
            model.Category = nimUser.Category;

            entities.SaveChanges();

            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminNimUserIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        /// <summary>
        /// 修改用户,只修改基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult NimUserUpdateInfo(Int32 id)
        {
            NimUser NimUser = entities.NimUser.Find(id);
            ViewData.Model = NimUser.NimUserEx;
            return View();
        }

        [HttpPost]
        public ActionResult NimUserUpdateInfo(NimUserEx nimUser)
        {
            entities.Entry(nimUser).State = EntityState.Modified;
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminNimUserIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }


        #endregion

        #region 汉语水平

        public ActionResult HsLevelIndex()
        {
            PagedList<HsLevel> hsLevels = entities.HsLevel.OrderBy(o => o.Id).ToPagedList(1, 25);

            ViewBag.HSLevels = hsLevels;
            return View();
        }

        public ActionResult HsLevelCreate(Int32? id)
        {

            if (id == null)
            {
                ViewData.Model = new HsLevel() { Id = 0 };
            }
            else
            {
                ViewData.Model = entities.HsLevel.Find(id);
            }

            return View();
        }
        [HttpPost]
        public ActionResult HsLevelCreate(HsLevel hsLevel)
        {
            if (hsLevel.Id > 0)
            {
                entities.Entry(hsLevel).State = EntityState.Modified;
            }
            else
            {
                entities.Entry(hsLevel).State = EntityState.Added;
            }
            entities.SaveChanges();
            return Json(new { statusCode = "200", message = "操作成功", navTabId = "AdminHsLevelIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" });
        }

        [HttpPost]
        public ActionResult HsLevelDelete(Int32 id)
        {
            HsLevel hs = new HsLevel() { Id = id };
            entities.Entry(hs).State = EntityState.Deleted;
            entities.SaveChanges();
            return Json(new { statusCode = "200", message = "操作成功", navTabId = "AdminHsLevelIndex", rel = "", callbackType = "", forwardUrl = "" });
        }

        public ActionResult HsLevelExpand(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            int hsLevel = ConvertUtil.ToInt32(form["hsLevel"], 0);
            ViewBag.HsLevel = hsLevel;
            Expression<Func<Theme, bool>> predicate = o => true;
            if (hsLevel > 0)
            {
                predicate = o => o.HsLevelId == hsLevel;
            }
            else if (hsLevel == 0)
            {
                predicate = o => o.HsLevelId == null;
            }
            ViewBag.ExpandHsLevel = entities.Theme.Where(predicate).OrderBy(o => o.Id).ToPagedList(pageIndex, pageSize);

            List<SelectListItem> items = new List<SelectListItem>();
            List<HsLevel> l = entities.HsLevel.ToList();
            l.Insert(0, new HsLevel() { Id = -1, Name = "-请选择-" });
            l.Insert(1, new HsLevel() { Id = 0, Name = "未分级" });

            foreach (var item in l)
            {
                items.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name, Selected = item.Id == hsLevel });
            }

            ViewBag.HsLevelSelectListItem = items;


            ViewBag.HSLevels = l;

            return View();
        }

        public ActionResult HsLevelAppend(Int32 id)
        {
            ViewData.Model = entities.HsLevel.Find(id);
            return View();
        }
        [HttpPost]
        public ActionResult HsLevelAppend(Int32 id, List<Int32> ids)
        {
            //SqlParameter para = new SqlParameter("@hsLevelId", id);
            entities.Database.ExecuteSqlCommand("UPDATE Theme SET HsLevelId=null WHERE HsLevelId=@hsLevelId", new SqlParameter("@hsLevelId", id));
            if (ids != null)
            {
                String sqlIn = String.Join(",", ids);
                entities.Database.ExecuteSqlCommand("UPDATE Theme SET HsLevelId=@hsLevelId WHERE Id IN(" + sqlIn + ")", new SqlParameter("@hsLevelId", id));
            }

            return Json(new { statusCode = "200", message = "操作成功", navTabId = "AdminHsLevelIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" });
        }



        #endregion

        #region 主题管理

        public ActionResult ThemeIndex(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            ViewBag.Themes = entities.Theme.OrderBy(o => o.Id).ToPagedList(pageIndex, pageSize);
            return View();
        }

        public ActionResult ThemeCreate(Int32? id)
        {
            Theme theme;
            if (id != null && id > 0)
            {
                theme = entities.Theme.Find(id);
            }
            else
            {
                theme = new Theme() { Id = 0 };
            }

            ViewData.Model = theme;
            return View();
        }

        [HttpPost]
        public ActionResult ThemeCreate(Theme theme)
        {
            if (theme.Id > 0)
            {
                entities.Entry(theme).State = EntityState.Modified;
            }
            else
            {
                entities.Theme.Add(theme);
            }
            entities.SaveChanges();

            return Json(new { statusCode = "200", message = "操作成功", navTabId = "AdminThemeIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" });
        }

        [HttpPost]
        public ActionResult ThemeDelete(Int32? id)
        {
            if (id != null && id > 0)
            {
                Theme model = entities.Theme.Find(id);
                entities.Theme.Remove(model);
                entities.SaveChanges();
            }
            return Json(new { statusCode = "200", message = "操作成功", navTabId = "AdminThemeIndex", rel = "", callbackType = "", forwardUrl = "" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ThemeUpdateQuestion(Int32 id)
        {
            Theme theme = entities.Theme.Find(id);
            ViewData.Model = theme;
            return View();
        }

        [HttpPost]
        public ActionResult ThemeUpdateQuestion(Int32 id, List<Question> questions)
        {
            //context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new SqlParameter("@author",  userSuppliedAuthor));
            entities.Database.ExecuteSqlCommand("UPDATE Question SET ThemeId = null WHERE ThemeId = @ThemeId", new SqlParameter("@ThemeId", id));

            foreach (var item in questions)
            {
                item.ThemeId = id;
                if (item.Id > 0)
                {
                    entities.Entry(item).State = EntityState.Modified;
                }
                else
                {
                    entities.Question.Add(item);
                }
            }
            entities.SaveChanges();
            return Json(new { statusCode = "200", message = "操作成功", navTabId = "AdminThemeIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" });
        }



        #endregion

        #region 订单管理
        public ActionResult OrderIndex(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 25);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);
            ViewData.Model = entities.Orders.OrderByDescending(o => o.CreateTime).ToPagedList(pageIndex, pageSize);

            return View();
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
            FileInfo fileInfo = new FileInfo(Server.MapPath("~" + filePath));
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            Mp3FileReader reader = new Mp3FileReader(file.InputStream);
            double Duration = reader.TotalTime.TotalMilliseconds;

            fileName = file.FileName;
            file.SaveAs(fileInfo.FullName);
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


        #region 安卓管理
        public ActionResult AndroidIndex(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 25);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            //关键字处理
            Expression<Func<Android, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(form["keyword"]))
            {
                String keyword = form["keyword"];
                predicate = o => o.VersionName.Contains(keyword) || o.UpgradeInfo.Contains(keyword);
                ViewBag.KeyWord = form["keyword"];
            }

            ViewData.Model = entities.Android.Where(predicate).OrderByDescending(o => o.CreateDate).ToPagedList(pageIndex, pageSize);

            return View();
        }

        public ActionResult AndroidCreate(Int32? id)
        {
            Android android = entities.Android.Find(id);
            ViewData.Model = android ?? new Android() { Id = 0, VersionType = 0, CreateDate = DateTime.Now };
            return View();
        }

        [HttpPost]
        public ActionResult AndroidCreate(Android android)
        {
            if (android.Id > 0)
            {
                entities.Entry(android).State = EntityState.Modified;
            }
            else
            {
                android.CreateDate = DateTime.Now;
                entities.Android.Add(android);
            }

            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminAndroidIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        [HttpPost]
        public ActionResult AndroidDelete(Int32 id)
        {
            Android a = entities.Android.Find(id);
            entities.Android.Remove(a);
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminAndroidIndex", rel = "", callbackType = "", forwardUrl = "" };
            return Json(data);
        }
        #endregion


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
