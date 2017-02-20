using ChineseChat.Library;
using Newtonsoft.Json;
using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class NimUserController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();

        public ActionResult Select(String keyword, Int32? category, Int32? numPerPage, Int32? pageNum)
        {

            System.Linq.Expressions.Expression<Func<View_User, bool>> predicateKeyword = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicateKeyword = o => o.Username.Contains(keyword) || o.Nickname.Contains(keyword);
            }

            System.Linq.Expressions.Expression<Func<View_User, bool>> predicate = o => true;
            if (category != null)
            {
                predicate = o => o.Category == category;
            }

            ViewData.Model = entities.View_User.Where(predicateKeyword).Where(predicate).OrderBy(o => o.CreateDate).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewBag.Keyword = keyword;
            ViewBag.Category = category;
            return View();
        }

        public ActionResult BatchCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BatchCreate(NimUser[] users)
        {
            //if (users != null)
            //{
            //    foreach (var user in users)
            //    {
            //        //创建账号ACCID
            //        user.Accid = Guid.NewGuid().ToString().Replace("-", "");
            //        user.NimUserEx = new NimUserEx() { Coins = 0 };

            //        //同步云信
            //        String json = NimUtil.UserCreate(user.Accid, null, null, null);
            //        //处理返回消息,Token
            //        Answer a = JsonConvert.DeserializeObject<Answer>(json);
            //        if (a.code == 200)
            //        {
            //            user.Token = a.info.token;
            //        }

            //        //密码加密
            //        user.Password = EncryptionUtil.Md5Encode(user.Password);
            //        //创建时间
            //        user.CreateDate = DateTime.Now;
            //        //user.Category = 0;
            //        user.IsOnline = 0;
            //        //user.IsActive = 0;
            //        user.IsEnable = 0;

            //        //保存数据
            //        entities.NimUser.Add(user);
            //    }
            //    entities.SaveChanges();
            //}
            return Json(new ResponseModel() { statusCode = "200", message = "创建成功", navTabId = "MasterNimUserSelect", callbackType = "closeCurrent" });
        }

        [HttpPost]
        public ActionResult Freeze(Int32 id)
        {
            entities.Database.ExecuteSqlCommand("UPDATE [NimUser] SET [IsActive]=0,[IsOnline]=0,[IsEnable]=0 WHERE Id=@id", new SqlParameter("@id", id));
            return Json(new { statusCode = "200", message = "操作成功", navTabId = "MasterNimUserSelect", rel = "", callbackType = "", forwardUrl = "" });
        }

        public ActionResult UploadUsersExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadUsersExcel(HttpPostedFileBase excelFile)
        {
            List<NimUser> users = new List<NimUser>();
            for (int i = 0; i < 25; i++)
            {
                users.Add(new NimUser() { Id = i, Username = "Username" + i, Password = "Password" + i });
            }



            return Json(users.Select(o => new { o.Id, o.Username, o.Password, o.Category, o.IsActive }));
        }
    }
}