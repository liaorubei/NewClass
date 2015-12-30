using ChineseChat.Library;
using Newtonsoft.Json;
using StudyOnline.Models;
using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace StudyOnline.Areas.Api.Controllers
{
    public class NimUserController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns>目前一直返回12306</returns>
        [HttpPost]
        public ActionResult GetCode(String phone)
        {
            return Json(new { code = 200, desc = "", info = new { captcha = "12306" } });
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="captcha">验证码</param>
        /// <returns>如果验证成功,返回code=200</returns>
        [HttpPost]
        public ActionResult Verify(String phone, String captcha)
        {
            if (String.IsNullOrEmpty(phone) || String.IsNullOrEmpty(captcha))
            {
                return Json(new { code = "20001", desc = "不能为空" });
            }

            NimUser user = entities.NimUser.FirstOrDefault(o => o.Username == phone);
            if (user != null)
            {
                return Json(new { code = "20001", desc = "该手机号码已经注册" });
            }

            if ("12306" != captcha)
            {
                return Json(new { code = "20001", desc = "验证码错误" });
            }

            return Json(new { code = 200, desc = "验证成功", info = "" });
        }

        /// <summary>
        /// 创建新用户
        /// </summary>
        /// <param name="username">用户名,手机号码</param>
        /// <param name="password">密码</param>
        /// <param name="nickname">昵称</param>
        /// <param name="category">用户类型,1是老师,0是学生</param>
        /// <returns>如果成功,返回code=200</returns>
        [HttpPost]
        public ActionResult Create(String username, String password, String nickname, Int32 category)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                return Json(new { code = 20001, desc = "参数不能为空" });
            }

            NimUser user = new NimUser();
            user.Accid = Guid.NewGuid().ToString().Replace("-", "");
            user.Username = username;
            user.Password = EncryptionUtil.Md5Encode(password);
            user.Category = category;
            user.CreateDate = DateTime.Now;

            String json = NimUtil.UserCreate(user.Accid, null, null, HttpUtility.UrlEncode(nickname));
            Answer a = JsonConvert.DeserializeObject<Answer>(json);
            if (a.code != 200)
            {
                return Json(new { code = a.code, desc = a.desc });
            }

            try
            {
                user.Token = a.info.token;
                user.NimUserEx = new NimUserEx() { Name = nickname };
                entities.NimUser.Add(user);
                entities.SaveChanges();
                return Json(new { code = 200, desc = "添加成功", info = new { user.Id, user.Accid, user.Token } });
            }
            catch (Exception ex)
            {
                return Json(new { code = 20002, desc = ex.Message, info = "" });
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(NimUserEx info)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名,即手机号码</param>
        /// <param name="password">密码</param>
        /// <returns>如果成功,返回code=200,并且返回云信帐号信息</returns>
        [HttpPost]
        public ActionResult Signin(String username, String password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                return Json(new { code = 20001, desc = "参数不能为空" });
            }

            NimUser user = entities.NimUser.FirstOrDefault(o => o.Username == username);

            if (user == null)
            {
                return Json(new { code = 20001, desc = "用户名不存在" });
            }

            if (!EncryptionUtil.VerifyMd5(password, user.Password))
            {
                return Json(new { code = 20001 });
            }
            return Json(new { code = 200, desc = "登录成功", info = new { user.Id, user.Accid, user.Token } });
        }

        [HttpPost]
        public ActionResult Select(String keyWord, Int32 skip, Int32 take)
        {
            Expression<Func<NimUser, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(keyWord))
            {
                predicate = o => o.Username.Contains(keyWord) || o.NimUserEx.Name.Contains(keyWord);
            }
            IQueryable<NimUser> users = entities.NimUser.Where(predicate).OrderBy(o => o.Username).Skip(skip).Take(take);
            return Json(new { code = 200, desc = "查询成功", info = users.Select(o => new { o.Id, o.Accid, o.Username, NickName = o.NimUserEx.Name, o.Category }) });
        }

        [HttpPost]
        public ActionResult Accept(Int32 id, Int32 target)
        {
            NimUser teacher = entities.NimUser.Find(id);
            teacher.IsEnable = 0;

            //添加对话记录
            entities.SaveChanges();
            return Json(new { code = 200, desc = "", info = "成功" });
        }

        [HttpPost]
        public ActionResult Refresh(Int32 id)
        {
            return Enqueue(id, true);
        }

        /// <summary>
        /// 教师入队
        /// </summary>
        /// <param name="id"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enqueue(Int32 id, bool refresh = false)
        {
            Int64 now = DateTime.Now.Ticks;

            NimUser user = entities.NimUser.Find(id);
            user.IsOnline = 1;
            user.IsEnable = 1;
            user.Refresh = now;

            //
            if (!refresh)
            {
                user.Enqueue = now;
            }
            entities.SaveChanges();

            //默认category=1为老师  //要求是老师,在线,可用
            Expression<Func<NimUser, bool>> predicate = o => o.IsOnline == 1 && o.IsEnable == 1 && o.Category == 1 && o.Enqueue <= user.Enqueue && o.Refresh >= (user.Refresh - 3000000000L);

            var teachers = entities.NimUser.Where(predicate).OrderBy(o => o.Enqueue);
            var temp = teachers.Skip(0).Take(5);
            return Json(new { code = 200, desc = "排队成功", info = new { Data = temp.Select(o => new { o.Id, o.Accid, o.NimUserEx.Name, o.Username, o.Category }), Rank = teachers.Count() } });
        }

        // 请求老师
        [HttpPost]
        public ActionResult ObtainTeacher(Int32 id)
        {
            Int64 now = DateTime.Now.Ticks - 3000000000L;

            NimUser student = entities.NimUser.Find(id);

            if (student == null)
            {
                return Json(new { code = 2001, desc = "没有这个学生" });
            }
            student.IsOnline = 1;
            student.Refresh = DateTime.Now.Ticks;

            //要求是老师,在线,可用
            NimUser teacher = entities.NimUser.Where(o => o.Category == 1 && o.IsOnline == 1 && o.IsEnable == 1).OrderBy(o => o.Enqueue).FirstOrDefault();// && o.LastRefresh > now);
            if (teacher == null)
            {
                return Json(new { code = 2001, desc = "暂时没有老师" });
            }
            teacher.IsEnable = 0;
            entities.SaveChanges();
            return Json(new { code = 200, desc = "获取成功", info = new { teacher.Id, teacher.Accid, teacher.NimUserEx.Name, teacher.Username } });
        }

        [HttpPost]
        public ActionResult ChooseTeacher(Int32 id, Int32 target)
        {

            Int64 now = DateTime.Now.Ticks - 3000000000L;
            NimUser student = entities.NimUser.Find(id);
            if (student == null)
            {
                return Json(new { code = 2001, desc = "没有这个学生" });
            }
            student.IsOnline = 1;
            student.Refresh = DateTime.Now.Ticks;

            NimUser teacher = entities.NimUser.Find(target);
            if (teacher == null)
            {
                return Json(new { code = 2001, desc = "没有这个老师" });
            }

            teacher.IsEnable = 0;
            entities.SaveChanges();
            return Json(new { code = 200, desc = "选择成功", info = new { teacher.Id, teacher.Accid, teacher.NimUserEx.Name, teacher.Username } });
        }

        [HttpPost]
        public ActionResult TeacherInqueue(int skip, int take)
        {
            Int64 now = DateTime.Now.Ticks;
            long refresh = now - 3000000000L;//5分钟轮循时间

            Expression<Func<NimUser, bool>> predicate = o => o.IsOnline == 1 && o.IsEnable == 1 && o.Category == 1 && (o.Enqueue < now) && (o.Refresh > refresh);//默认category=1为老师  //要求是老师,在线,可用
            Expression<Func<NimUser, long?>> keySelector = o => o.Enqueue;
            List<NimUser> teachers = entities.NimUser.Where(predicate).OrderBy(keySelector).Skip(skip).Take(take).ToList();
            return Json(new { code = 200, desc = "查询成功", info = teachers.Select(o => new { o.Id, o.Accid, o.NimUserEx.Name, o.Username, o.Category }) });
        }

    }
}
