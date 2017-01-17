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
using System.Text;
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
            List<X_Menu> menus = entities.X_Menu.Where(o => o.ParentId == null).ToList();
            ViewBag.Menus = menus;
            return View();
        }

        public ActionResult Account()
        {
            ViewData.Model = Session["CurrentUser"];
            return View();
        }

        [HttpPost]
        public ActionResult Account(X_User user)
        {
            try
            {
                if (!String.IsNullOrEmpty(user.Id))
                {
                    X_User x = entities.X_User.Find(user.Id);
                    x.Username = user.Username;
                    x.Password = user.Password;
                    x.Nickname = user.Nickname;
                    x.Truename = user.Truename;

                    Session["CurrentUser"] = x;
                    entities.SaveChanges();
                }
                return Json(new ResponseModel() { statusCode = "200", message = "操作成功", callbackType = "closeCurrent" });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel() { statusCode = "300", message = ex.Message });
            }
        }

        public ActionResult Player() { return View(); }

        #region 文档相关
        public ActionResult DocsList(String keyword, Int32? levelId, Int32? folderId, Int32? pageNum, Int32? numPerPage, String orderField, String orderDirection)
        {
            //检索处理
            Expression<Func<View_Document_Lite, bool>> predicateKeyWord = d => true;
            Expression<Func<View_Document_Lite, bool>> predicateLevelId = d => true;
            Expression<Func<View_Document_Lite, bool>> predicateFolderId = o => true;

            if (!String.IsNullOrEmpty(keyword))
            {
                predicateKeyWord = o => o.Title.Contains(keyword) || o.TitleSubCn.Contains(keyword);
            }

            if (levelId.HasValue && levelId > 0)
            {
                predicateLevelId = o => o.LevelId == levelId;
            }

            if (folderId.HasValue && folderId > 0)
            {
                predicateFolderId = o => o.FolderId == folderId;
            }

            Func<View_Document_Lite, object> keySelector = o => o.Sort;
            if (!String.IsNullOrEmpty(orderField))
            {
                if ("AddDate".Equals(orderField))
                {
                    keySelector = o => o.AddDate;
                }
                else if ("AuditDate".Equals(orderField))
                {
                    keySelector = o => o.AuditDate;
                }
                else if ("Sort".Equals(orderField))
                {
                    keySelector = o => o.Sort;
                }
            }

            //数据和分页检索条件处理
            PagedList<View_Document_Lite> docs = entities.View_Document_Lite.Where(predicateKeyWord).Where(predicateLevelId).Where(predicateFolderId).OrderByDescending(keySelector).ToPagedList(pageNum ?? 0, numPerPage ?? 20);
            ViewBag.Docs = docs;

            List<Level> levels = entities.Level.OrderBy(o => o.Name).ToList();
            List<Folder> folders = entities.Folder.Where(o => o.TargetId == null).OrderBy(o => o.Name).ToList();

            ViewBag.Levels = levels;
            ViewBag.Folders = folders;
            ViewBag.KeyWord = keyword;//关键字
            ViewBag.LevelId = levelId;//文章级别
            ViewBag.FolderId = folderId;//文件夹
            ViewBag.OrderField = orderField;
            return View();
        }

        public ActionResult DocsLookup(FormCollection form)
        {
            //分页处理
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 20);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            //检索处理
            Expression<Func<Document, bool>> predicateKeyWord = d => true;
            Expression<Func<Document, bool>> predicateLevelId = d => true;
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

        /// <summary>
        /// 创建文档界面,选择文件夹功能的附加功能,因为文件夹太多了,所以使用查找带回功能,
        /// </summary>
        /// <returns></returns>
        public ActionResult FolderTreeLookUp()
        {
            ViewData.Model = entities.Folder.Where(o => o.ParentId == null).OrderBy(o => o.Sort).ThenBy(o => o.Name).ToList();
            return View();
        }

        public ActionResult DocsCreate(Int32? id)
        {
            Document document = id == null ? new Document() { Id = 0, Folder = new Folder() } : entities.Document.FirstOrDefault(d => d.Id == id);
            ViewData.Model = document;

            List<Level> levels = entities.Level.OrderBy(o => o.Sort).ToList();
            List<SelectListItem> selectListLevels = new List<SelectListItem>();
            selectListLevels.Add(new SelectListItem() { Value = "", Text = "-请选择-" });
            foreach (var item in levels)
            {
                selectListLevels.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name, Selected = document.LevelId == item.Id });
            }
            ViewBag.Levels = selectListLevels;

            List<SelectListItem> selectListCourse = new List<SelectListItem>();
            selectListCourse.Add(new SelectListItem() { Value = "", Text = "-请选择-" });
            selectListCourse.Add(new SelectListItem() { Value = "1", Text = "普通", Selected = 1 == document.Category });
            selectListCourse.Add(new SelectListItem() { Value = "2", Text = "课程", Selected = 2 == document.Category });
            selectListCourse.Add(new SelectListItem() { Value = "3", Text = "新闻", Selected = 3 == document.Category });

            ViewBag.Course = selectListCourse;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DocsCreate(Document document)
        {
            if (document.Id > 0)
            {
                Document model = entities.Document.FirstOrDefault(d => d.Id == document.Id);
                model.LevelId = document.LevelId;
                model.FolderId = document.FolderId;
                model.Title = document.Title;
                model.TitleTwo = document.TitleTwo;//文件的标题（翻译）
                model.TitlePy = document.TitlePy;
                model.TitleSubCn = document.TitleSubCn;
                model.TitleSubEn = document.TitleSubEn;
                model.TitleSubPy = document.TitleSubPy;
                model.Category = document.Category;
                model.Lyrics = document.Lyrics;
                model.SoundPath = document.SoundPath;//音频路径
                model.Length = document.Length;//文件长度
                model.LengthString = document.LengthString;//音频文件的播放长度
                model.AuditCase = document.AuditCase;//审核情况
                model.AuditDate = document.AuditDate;//审核日期
                model.Cover = Request.Form["Cover.FilePath"];//封面,查找带回
            }
            else
            {
                document.AddDate = DateTime.Now;
                document.Cover = Request.Form["Cover.FilePath"];//封面,查找带回
                entities.Document.Add(document);
            }
            entities.SaveChanges();

            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminDocsList", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        /// <summary>
        /// 给文档添加排序功能
        /// </summary>
        /// <param name="id">文档Id</param>
        /// <param name="sort">序号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DocumentSort(Int32 id, Double? sort)
        {
            entities.Document.Find(id).Sort = sort;
            entities.SaveChanges();
            return Json(new { message = "操作完成" });
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

        [HttpPost]
        public ActionResult BatchDocumentDelete(Int32[] ids)
        {
            if (ids != null)
            {
                entities.Database.ExecuteSqlCommand(String.Format("DELETE FROM [Document] WHERE Id IN({0})", String.Join(",", ids)));
            }
            return Json(new ResponseModel() { statusCode = "200", message = "批量删除成功", rel = "AdminDocsList", navTabId = "AdminDocsList" });
        }

        [HttpPost]
        public ActionResult BatchDocumentAudit(Int32[] ids)
        {
            if (ids != null)
            {
                StringBuilder builder = new StringBuilder();
                DateTime now = DateTime.Now;
                foreach (var item in ids)
                {
                    builder.AppendLine(String.Format("UPDATE [Document] SET [AuditCase]=2,[AuditDate]='{0}' WHERE Id={1}", now.ToString("yyyy-MM-dd HH:mm:ss"), item));
                }
                entities.Database.ExecuteSqlCommand(builder.ToString());
            }
            return Json(new ResponseModel() { statusCode = "200", message = "批量审核成功", rel = "AdminDocsList", navTabId = "AdminDocsList" });
        }

        /// <summary>
        /// 批量编辑,为了方便批量编辑排序,一级标题,二级标题,三级标题
        /// </summary>
        /// <returns></returns>
        public ActionResult DocumentBatchUpdate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DocumentBatchUpdate(Document[] documents)
        {
            return Json(new ResponseModel() { });
        }

        #endregion

        #region 分类相关
        public ActionResult LevelList(FormCollection form)
        {
            PagedList<Level> levels = entities.Level.OrderBy(l => l.Sort).ToPagedList(1, 25);
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
                entities.Entry(level).State = EntityState.Modified;
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
            String keyword = form["keyword"];
            Expression<Func<Folder, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicate = o => o.Name.Contains(keyword);
            }
            ViewBag.Keyword = form["keyword"];

            //等级
            int levelId = ConvertUtil.ToInt32(form["levelId"], -1);
            Expression<Func<Folder, bool>> predicateLevel = o => true;
            if (levelId > 0)
            {
                predicateLevel = o => o.LevelId == levelId;
            }

            List<Level> levels = entities.Level.ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "-请选择-", Value = "-1" });
            foreach (var item in levels)
            {
                items.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString(), Selected = levelId == item.Id });
            }
            ViewBag.Levels = items;

            PagedList<Folder> folders = entities.Folder.Where(predicate).Where(predicateLevel).OrderBy(o => o.LevelId).ThenBy(o => o.ParentId).ThenBy(o => o.Sort).ToPagedList(pageIndex, pageSize);
            ViewBag.Folders = folders;
            return View();
        }

        public ActionResult FolderCreate(Int32? id)
        {
            List<Level> levels = entities.Level.ToList();
            ViewBag.Levels = levels;
            ViewData.Model = id == null ? new Folder() : entities.Folder.Find(id);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNum"></param>
        /// <param name="numPerPage"></param>
        /// <param name="keyword"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public ActionResult FolderLookup(Int32 id, Int32? pageNum, Int32? numPerPage, String keyword, String mode)
        {
            Expression<Func<Folder, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicate = o => o.Name.Contains(keyword);
            }

            ViewData.Model = entities.Folder.Where(o => o.Id != id).Where(predicate).OrderBy(o => o.LevelId).ThenBy(o => o.Sort).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewBag.Id = id;
            ViewBag.Keyword = keyword;
            ViewBag.Mode = mode;
            return View();
        }

        [HttpPost]
        public ActionResult FolderCreate(Folder folder)
        {
            if (folder.Id > 0)
            {
                entities.Entry(folder).State = EntityState.Modified;
            }
            else
            {
                entities.Folder.Add(folder);
            }
            entities.SaveChanges();

            var data = new { statusCode = "200", message = "操作成功", navTabId = "AdminFolderIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        [HttpPost]
        public ActionResult FolderSort(Int32 id, Int32 sort)
        {
            entities.Folder.Find(id).Sort = sort;
            entities.SaveChanges();

            return Json(new ResponseModel() { statusCode = "200", message = "操作成功" });
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

        public ActionResult Recharge(Int32 id)
        {
            NimUser user = entities.NimUser.Find(id);
            ViewData.Model = user;
            return View();
        }

        [HttpPost]
        public ActionResult Recharge(Int32 id, Int32 coins)
        {
            NimUserEx ex = entities.NimUserEx.Find(id);
            if (ex.NimUser.Category == 1)
            {
                return Json(new { statusCode = "300", message = "教师不能充值" });
            }
            ex.Coins = coins;
            entities.SaveChanges();
            return Json(new { statusCode = "200", message = "操作成功", navTabId = "AdminNimUserIndex", rel = "", callbackType = "closeCurrent", forwardUrl = "" });
        }



        #endregion

        #region 教师管理
        public ActionResult TeacherIndex()
        {
            Expression<Func<NimUser, bool>> predicate = o => true;
            int pageIndex = 0;
            int pageSize = 25;
            var model = entities.NimUser.Where(o => o.Category == 1).OrderByDescending(o => o.CreateDate).Where(predicate).ToPagedList(pageIndex, pageSize);

            ViewData.Model = model;
            return View();
        }

        public ActionResult TeacherUpdate(Int32 id)
        {
            NimUserEx user = entities.NimUserEx.Find(id);
            ViewData.Model = user;
            return View();
        }

        [HttpPost]
        public ActionResult TeacherUpdate(NimUserEx ex)
        {
            NimUserEx model = entities.NimUserEx.Find(ex.Id);
            return View();
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
            int pageSize = ConvertUtil.ToInt32(form["numPerPage"], 25);
            int pageIndex = ConvertUtil.ToInt32(form["pageNum"], 1);

            Expression<Func<Theme, bool>> keyword = o => true;
            if (!String.IsNullOrEmpty(form["keyword"]))
            {
                String k = form["keyword"] + "";
                keyword = o => o.Name.Contains(k);
            }
            ViewBag.Kerword = form["keyword"];
            ViewBag.Themes = entities.Theme.Where(keyword).OrderBy(o => o.Sort).ToPagedList(pageIndex, pageSize);
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

            var i = entities.HsLevel.ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in i)
            {
                items.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name, Selected = theme.HsLevelId == item.Id });
            }
            ViewBag.SelectListItem = items;

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

        #region 菜单管理
        public ActionResult MenuIndex()
        {
            var menus = entities.X_Menu.Where(o => o.ParentId == null).ToList();
            ViewData.Model = menus;
            return View();
        }

        public ActionResult MenuLookup()
        {
            return View();





        }

        public ActionResult MenuCreate(Int32 id)
        {
            Int32? parentId = null;
            if (id > 0)
            {
                parentId = id;
            }
            ViewData.Model = new X_Menu() { ParentId = parentId };
            return View();
        }

        [HttpPost]
        public ActionResult MenuCreate(X_Menu menu)
        {
            entities.X_Menu.Add(menu);
            entities.SaveChanges();
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
            Expression<Func<Android, bool>> predicateKeyword = o => true;
            if (!String.IsNullOrEmpty(form["keyword"]))
            {
                String keyword = form["keyword"];
                predicateKeyword = o => o.VersionName.Contains(keyword) || o.UpgradeInfo.Contains(keyword);
                ViewBag.KeyWord = form["keyword"];
            }

            Expression<Func<Android, bool>> predicate = o => true;
            Int32 versionType = ConvertUtil.ToInt32(form["searchBarVersionType"], -1);
            if (versionType > -1)
            {
                predicate = o => o.VersionType == versionType;
            }


            ViewData.Model = entities.Android.Where(predicateKeyword).Where(predicate).OrderByDescending(o => o.CreateDate).ToPagedList(pageIndex, pageSize);

            List<SelectListItem> selectListItemVersionType = new List<SelectListItem>();
            selectListItemVersionType.Add(new SelectListItem() { Text = "-请选择-", Value = "" });

            var c = typeof(Client);
            var names = Enum.GetValues(c);
            foreach (Int32 item in names)
            {
                selectListItemVersionType.Add(new SelectListItem() { Text = Enum.GetName(c, item), Value = item.ToString(), Selected = item == versionType });
            }
            ViewBag.VersionTypes = selectListItemVersionType;

            return View();
        }

        public ActionResult AndroidCreate(Int32? id)
        {
            Android android = entities.Android.Find(id) ?? new Android() { Id = 0, VersionType = -1, CreateDate = DateTime.Now };
            ViewData.Model = android;

            List<SelectListItem> selectListItemVersionType = new List<SelectListItem>();
            selectListItemVersionType.Add(new SelectListItem() { Text = "-请选择-", Value = "" });

            var c = typeof(Client);
            var names = Enum.GetValues(c);
            foreach (Int32 item in names)
            {
                selectListItemVersionType.Add(new SelectListItem() { Text = Enum.GetName(c, item), Value = item.ToString(), Selected = item == android.VersionType });
            }
            ViewBag.VersionTypes = selectListItemVersionType;

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


        #region 单文件上传
        /// <summary>
        /// 单文件上传界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            return View();
        }

        /// <summary>
        /// 单文件上传回调
        /// </summary>
        /// <param name="file">文件实体,要求表单名称为file</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            //相对路径
            String path = String.Format("/File/{0}/{1}/{2}", DateTime.Now.ToString("yyyyMMdd"), Guid.NewGuid(), Path.GetExtension(file.FileName));

            //绝对路径
            FileInfo fileInfo = new FileInfo(Server.MapPath("~" + path));
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            //保存文件到绝对路径
            file.SaveAs(fileInfo.FullName);

            //数据返回
            return Json(new { FileName = file.FileName, FilePath = path, FullPath = fileInfo.FullName, FileSize = file.ContentLength });
        }

        #endregion


    }
}
