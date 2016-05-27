using ChineseChat.Library;
using Newtonsoft.Json;
using StudyOnline.Models;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Data.Entity;
using System.IO;
using System.Text.RegularExpressions;
using StudyOnline.Utils;
using System.Data.SqlClient;

namespace StudyOnline.Areas.Api.Controllers
{
    public class NimUserController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="email">手机号码</param>
        /// <returns>目前一直返回12306</returns>
        [HttpPost]
        public ActionResult GetCode(String email)
        {
            Regex regex = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            if (!regex.IsMatch(email))
            {
                return Json(new { code = 20001, desc = "E-mail address invalid" });
            }

            if (!entities.NimUser.Any(o => o.Username == email))
            {
                return Json(new { code = 201, desc = "你所输入的帐号不存在", info = "严肃点,好不" });
            }

            //写入数据库
            //生成随机值
            Random random = new Random();
            int code = random.Next(1000, 9999);

            entities.Database.ExecuteSqlCommand("update authcode set code=null where contact=@contact", new SqlParameter("@contact", email));

            AuthCode auth = new AuthCode();
            auth.Id = Guid.NewGuid().ToString().Replace("-", "");
            auth.Contact = email;
            auth.Code = code + "";
            auth.Createtime = DateTime.Now;
            entities.AuthCode.Add(auth);
            entities.SaveChanges();

            //发送到邮箱
            try
            {
                string senderServerIp = "smtp-n.global-mail.cn";
                string mailPort = "25";
                string toMailAddress = email;
                string fromMailAddress = "Service@chinesechat.cn";//公司服务邮箱
                string subjectInfo = "Your ChineseChat verification code";
                string bodyInfo = "Your verification code is:" + code + ".Please enter it in the corresponding blank on Reset Password page within ten minutes.";
                string mailUsername = "Service@chinesechat.cn";
                string mailPassword = "60190466hwdfKF"; //发送邮箱的密码（）

                MyEmail my = new MyEmail(senderServerIp, toMailAddress, fromMailAddress, subjectInfo, bodyInfo, mailUsername, mailPassword, mailPort, false, false);
                my.Send();
            }
            catch (Exception ex)
            {
                //返回信息
                return Json(new { code = 201, desc = "generate verification code failure", info = ex.Message });
            }

            //返回信息
            return Json(new { code = 200, desc = "请求成功", info = "验证码已经发送到你的邮箱" });
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="contact">联系方式</param>
        /// <param name="captcha">验证码</param>
        /// <returns>如果验证成功,返回code=200</returns>
        [HttpPost]
        public ActionResult Verify(String contact, String captcha)
        {
            if (String.IsNullOrEmpty(contact) || String.IsNullOrEmpty(captcha))
            {
                return Json(new { code = 203, desc = "e-mail or verification code cannot be empty!" });
            }

            AuthCode authcode = entities.AuthCode.OrderByDescending(o => o.Createtime).FirstOrDefault(o => o.Contact == contact && o.Code == captcha);
            if (authcode == null)
            {
                return Json(new { code = 202, desc = "e-mail or verification code not found" });
            }

            var span = DateTime.Now - authcode.Createtime.Value;

            if (span.TotalSeconds > (30 * 60))
            {
                return Json(new { code = 201, desc = "verification code expired" });
            }

            {
                return Json(new { code = 200, desc = "Verify success" });
            }
        }

        /// <summary>
        /// 修改密码,直接修改为指定密码,不用验证旧密码
        /// </summary>
        /// <param name="username">邮箱或者用户名,默认邮箱</param>
        /// <param name="password">目标密码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(String username, String password)
        {
            NimUser user = entities.NimUser.Single(o => o.Username == username);
            if (user == null)
            {
                return Json(new { code = 201, desc = "username not found" });
            }

            if ((password + "").Length < 8)
            {
                return Json(new { code = 201, desc = "password can not be less than 8 character" });
            }

            user.Password = EncryptionUtil.Md5Encode(password);//密码MD5加密
            entities.SaveChanges();

            return Json(new { code = 200, desc = "修改成功", info = new { user.Id, user.Accid, user.Token, user.Username } });
        }

        /// <summary>
        /// 修改密码,要求有用户名,旧密码,新密码
        /// </summary>
        /// <param name="username">邮箱或者用户名,默认邮箱</param>
        /// <param name="old_password">目标密码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyPassword(String username, String old_password, String new_password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(old_password) || String.IsNullOrEmpty(new_password))
            {
                return Json(new { code = 201, desc = "username or password cannot be empty" });
            }

            if (old_password.Length < 8 || new_password.Length < 8)
            {
                return Json(new { code = 201, desc = "password cannot be less than 8 characters" });
            }

            if (old_password == new_password)
            {
                return Json(new { code = 201, desc = "the old password and the new password are the same" });
            }


            try
            {
                NimUser user = entities.NimUser.SingleOrDefault(o => o.Username == username);

                if (user == null)
                {
                    return Json(new { code = 201, desc = "username not found" });
                }

                if (user.Password != EncryptionUtil.Md5Encode(old_password))
                {
                    return Json(new { code = 201, desc = "incorrect password" });
                }


                user.Password = EncryptionUtil.Md5Encode(new_password);//密码MD5加密
                entities.SaveChanges();

                return Json(new
                {
                    code = 200,
                    desc = "修改成功",
                    info = new
                    {
                        user.Id,
                        user.Accid,
                        user.Token,
                        user.Username,
                        Nickname = user.NimUserEx.Name
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }

        }

        /// <summary>
        /// 创建新用户
        /// </summary>
        /// <param name="email">用户名,手机号码</param>
        /// <param name="password">密码</param>
        /// <param name="category">用户类型,1是老师,0是学生</param>
        /// <returns>如果成功,返回code=200</returns>
        [HttpPost]
        public ActionResult Create(String email, String password, Int32 category)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return Json(new { code = 20001, desc = "email or password cannot be empty" });
            }

            Regex regex = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            if (!regex.IsMatch(email))
            {
                return Json(new { code = 20001, desc = "E-mail address invalid" });
            }

            if (password.Length < 8)
            {
                return Json(new { code = 20001, desc = "password cannot be less than 8 characters" });
            }

            if (entities.NimUser.Any(o => o.Username == email || o.NimUserEx.Email == email))
            {
                return Json(new { code = 20001, desc = "username already exists" });
            }

            NimUser user = new NimUser();
            user.Accid = Guid.NewGuid().ToString().Replace("-", "");
            user.Username = email;
            user.Password = EncryptionUtil.Md5Encode(password);
            user.Category = category;
            user.CreateDate = DateTime.Now;

            String json = NimUtil.UserCreate(user.Accid, null, null, HttpUtility.UrlEncode(email));
            Answer a = JsonConvert.DeserializeObject<Answer>(json);
            if (a.code != 200)
            {
                return Json(new { code = a.code, desc = a.desc });
            }

            try
            {
                user.Token = a.info.token;
                user.NimUserEx = new NimUserEx() { Email = email, Name = email };
                entities.NimUser.Add(user);
                entities.SaveChanges();
                return Json(new { code = 200, desc = "创建成功", info = new { user.Id, user.Accid, user.Token } });
            }
            catch (Exception ex)
            {
                return Json(new { code = 20002, desc = "创建失败" + ex.Message });
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(int id, String Name, HttpPostedFileBase icon, String Email, DateTime? Birth, String Mobile, Int32? Gender, String Country, String Language, String Job, HttpPostedFileBase Voice, String About, String School)
        {
            NimUserEx info = entities.NimUserEx.Find(id);
            //  NimUserEx info = new NimUserEx() { Icon = "默认图片" };
            if (info == null)
            {
                return Json(new { code = 201, desc = "用户不存在", info = info });
            }

            //处理图片文件
            if (icon != null)
            {
                String filePath = String.Format("/{0}/{1}/{2}{3}", "File", DateTime.Now.ToString("yyyyMMdd"), Guid.NewGuid(), Path.GetExtension(icon.FileName));
                FileInfo file = new FileInfo(Server.MapPath("~" + filePath));
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                icon.SaveAs(file.FullName);
                info.Icon = filePath;
            }

            //处理音频文件
            if (Voice != null)
            {

                String filePath = String.Format("/{0}/{1}/{2}{3}", "File", DateTime.Now.ToString("yyyyMMdd"), Guid.NewGuid(), Path.GetExtension(Voice.FileName));
                FileInfo file = new FileInfo(Server.MapPath("~" + filePath));
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }

                Voice.SaveAs(file.FullName);
                info.Voice = filePath;
            }

            if (Email != null)
            {
                info.Email = Email;
            }
            if (Name != null)
            {
                info.Name = Name;
            }
            if (Birth != null)
            {
                info.Birth = Birth;
            }
            if (Mobile != null)
            {
                info.Mobile = Mobile;
            }
            if (Gender != null)
            {
                info.Gender = Gender;
            }
            if (Country != null)
            {
                info.Country = Country;
            }
            if (Language != null)
            {
                info.Language = Language;
            }
            if (Job != null)
            {
                info.Job = Job;
            }
            if (About != null)
            {
                info.About = About;
            }
            if (School != null)
            {
                info.School = School;
            }


            try
            {
                entities.SaveChanges();
                return Json(new
                {
                    code = 200,
                    desc = "修改成功",
                    info = new
                    {
                        info.Id,
                        info.Name,
                        info.Icon,
                        Avatar = info.Icon,
                        info.Email,
                        Birth = (info.Birth == null ? "" : info.Birth.Value.ToString("yyyy-MM-dd")),
                        info.Mobile,
                        info.Gender,
                        NickName = info.Name,
                        Nickname = info.Name,
                        info.Country,
                        info.Language,
                        info.Job,
                        info.About,
                        info.Coins,
                        info.School,
                        info.NimUser.Username,
                        info.NimUser.Accid,
                        info.NimUser.Token
                    }
                });
            }
            catch
            {
                return Json(new
                {
                    code = 201,
                    desc = "修改失败",
                    info = new
                    {
                        info.Id,
                        info.Name,
                        info.Icon,
                        info.Email,
                        Birth = (info.Birth == null ? "" : info.Birth.Value.ToString("yyyy-MM-dd")),
                        info.Mobile,
                        info.Gender
                    }
                });
            }
        }

        [HttpPost]
        public ActionResult UpdatePhotos(String username, List<String> deletedPhotos, List<HttpPostedFileBase> newPhotos)
        {
            try
            {
                bool isUpdate = false;
                if (!entities.NimUser.Any(o => o.Username == username))
                {
                    return Json(new { code = 201, desc = "指定用户不存在" });
                }

                NimUser user = entities.NimUser.Single(o => o.Username == username);

                #region 旧照片处理
                if (deletedPhotos != null && deletedPhotos.Any())
                {
                    List<UploadFile> origin = user.UploadFile.ToList();
                    List<UploadFile> remove = new List<UploadFile>();

                    //选择要删除的
                    foreach (var item in origin)
                    {
                        if (deletedPhotos.Any(o => item.Path == o))
                        {
                            remove.Add(item);
                        }
                    }

                    //删除要删除的
                    foreach (var item in remove)
                    {
                        user.UploadFile.Remove(item);
                    }
                    isUpdate = true;
                }
                #endregion

                #region 新照片处理
                if (newPhotos != null && newPhotos.Any())
                {
                    foreach (var item in newPhotos)
                    {
                        user.UploadFile.Add(Helper.SaveUploadFile(Server, item));
                    }
                    isUpdate = true;
                }
                #endregion

                //保存数据
                if (isUpdate)
                {
                    entities.SaveChanges();
                    return Json(new { code = 200, desc = "更新成功", info = new { user.Id, Photos = user.UploadFile.Select(o => o.Path) } });
                }
                else
                {
                    return Json(new { code = 201, desc = "没有更新" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateStudent(String username, String nickname, String mobile, Int32? gender, DateTime? birth, String country, String language, String job, HttpPostedFileBase icon)
        {
            try
            {
                if (!entities.NimUser.Any(o => o.Username == username))
                {
                    return Json(new { code = 201, desc = "指定用户不存在" });
                }

                NimUser user = entities.NimUser.Single(o => o.Username == username);
                NimUserEx ex = user.NimUserEx;

                if (icon != null && icon.ContentLength > 0)
                {
                    ex.Icon = Helper.SaveUploadFile(this.Server, icon).Path;
                }

                ex.Name = nickname;
                ex.Mobile = mobile;
                ex.Gender = gender;
                ex.Birth = birth;
                ex.Country = country;
                ex.Language = language;
                ex.Job = job;

                entities.SaveChanges();
                return Json(new
                {
                    code = 200,
                    desc = "更新成功",
                    info = new
                    {
                        ex.Id,
                        Avatar = ex.Icon,
                        Nickname = ex.Name,
                        ex.Mobile,
                        ex.Gender,
                        Birth = ex.Birth == null ? null : ex.Birth.Value.ToString("yyyy-MM-dd"),
                        ex.Country,
                        ex.Language,
                        ex.Job,
                        Photos = user.UploadFile.Select(o => o.Path)
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateTeacher(String username, String nickname, String mobile, Int32? gender, DateTime? birth, String about, String school, String spoken, String hobbies, String country, List<String> deletedPhotos, HttpPostedFileBase icon)
        {
            try
            {
                if (!entities.NimUser.Any(o => o.Username == username))
                {
                    return Json(new { code = 201, desc = "指定用户不存在" });
                }

                NimUser user = entities.NimUser.Single(o => o.Username == username);
                NimUserEx ex = user.NimUserEx;
                ex.Name = nickname;
                ex.Mobile = mobile;
                ex.Gender = gender;
                ex.Birth = birth;
                ex.About = about;
                ex.School = school;
                ex.Spoken = spoken;
                ex.Hobbies = hobbies;
                ex.Country = country;


                if (icon != null && icon.ContentLength > 0)
                {
                    ex.Icon = Helper.SaveUploadFile(this.Server, icon).Path;
                }

                #region 旧照片处理
                if (deletedPhotos != null && deletedPhotos.Any())
                {
                    List<UploadFile> origin = user.UploadFile.ToList();
                    List<UploadFile> remove = new List<UploadFile>();

                    //选择要删除的
                    foreach (var item in origin)
                    {
                        if (deletedPhotos.Any(o => item.Path == o))
                        {
                            remove.Add(item);
                        }
                    }

                    //删除要删除的
                    foreach (var item in remove)
                    {
                        user.UploadFile.Remove(item);
                    }
                }
                #endregion

                #region 新照片处理
                var newPhoto = Request.Files.AllKeys.Where(o => o.Contains("newPhoto")).ToList();

                List<HttpPostedFileBase> kkk = new List<HttpPostedFileBase>();
                foreach (var item in newPhoto)
                {
                    kkk.Add(Request.Files[item]);
                }

                foreach (var item in kkk)
                {
                    user.UploadFile.Add(Helper.SaveUploadFile(Server, item));
                }
                #endregion

                entities.SaveChanges();
                return Json(new
                {
                    code = 200,
                    desc = "更新成功",
                    info = new
                    {
                        ex.Id,
                        Avatar = ex.Icon,
                        Nickname = ex.Name,
                        ex.Mobile,
                        ex.Gender,
                        Birth = ex.Birth == null ? null : ex.Birth.Value.ToString("yyyy-MM-dd"),
                        ex.School,
                        ex.Hobbies,
                        ex.Spoken,
                        ex.About,
                        ex.Country,
                        Photos = user.UploadFile.Select(o => o.Path)
                    }
                });

            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
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
            return Json(new
            {
                code = 200,
                desc = "登录成功",
                info = new
                {
                    user.Id,
                    user.Accid,
                    user.Token,
                    user.Username,
                    user.NimUserEx.Icon,
                    Avatar = user.NimUserEx.Icon,
                    user.NimUserEx.Name,
                    NickName = user.NimUserEx.Name,
                    Nickname = user.NimUserEx.Name,
                    user.NimUserEx.Gender,
                    user.NimUserEx.Email,
                    user.NimUserEx.Mobile,
                    Birth = (user.NimUserEx.Birth == null ? "" : user.NimUserEx.Birth.Value.ToString("yyyy-MM-dd")),
                    user.NimUserEx.Country,
                    user.NimUserEx.Language,
                    user.NimUserEx.Job,
                    user.NimUserEx.Voice,
                    user.NimUserEx.About,
                    user.NimUserEx.Coins,
                    user.NimUserEx.School,
                    user.NimUserEx.Spoken,
                    user.NimUserEx.Hobbies,
                    Photos = user.UploadFile.Select(o => o.Path).ToList()
                }
            });
        }

        /// <summary>
        /// 查询教师
        /// </summary>
        /// <param name="keyWord">关键字,用户名或者昵称</param>
        /// <param name="skip">跳过记录数</param>
        /// <param name="take">选择记录数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Select(String keyWord, Int32 skip, Int32 take)
        {
            Expression<Func<NimUser, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(keyWord))
            {
                predicate = o => o.Username.Contains(keyWord) || o.NimUserEx.Name.Contains(keyWord);
            }
            IQueryable<NimUser> users = entities.NimUser.Where(predicate).OrderBy(o => o.Username).Skip(skip).Take(take);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = users.Select(o => new
                {
                    o.Id,
                    o.Accid,
                    o.Username,
                    NickName = o.NimUserEx.Name,
                    Nickname = o.NimUserEx.Name,
                    o.Category
                })
            });
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>返回用户名</returns>
        [HttpPost]
        public ActionResult GetById(Int32 id)
        {
            NimUser user = entities.NimUser.Find(id);
            if (user == null)
            {
                return Json(new { code = 2001, desc = "没有这个人", info = new { Id = id } });
            }
            return Json(new
            {
                code = 200,
                desc = "",
                info = new
                {
                    user.Id,
                    user.Accid,
                    user.Username,
                    user.Category,
                    user.NimUserEx.Icon,
                    Avatar = user.NimUserEx.Icon,
                    user.NimUserEx.Name,
                    NickName = user.NimUserEx.Name,
                    Nickname = user.NimUserEx.Name,
                    user.NimUserEx.Gender,
                    user.NimUserEx.Email,
                    user.NimUserEx.Mobile,
                    Birth = (user.NimUserEx.Birth == null ? "" : user.NimUserEx.Birth.Value.ToString("yyyy-MM-dd")),
                    user.NimUserEx.Country,
                    user.NimUserEx.Language,
                    user.NimUserEx.Job,
                    user.NimUserEx.Voice,
                    user.NimUserEx.About,
                    user.NimUserEx.Coins,
                    user.NimUserEx.Score
                }
            });
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="accid">用户的Accid</param>
        /// <returns>返回用户名</returns>
        [HttpPost]
        public ActionResult GetByAccId(String accid)
        {
            NimUser user = entities.NimUser.FirstOrDefault(o => o.Accid == accid);
            if (user == null)
            {
                return Json(new { code = 2001, desc = "没有这个人", info = new { Accid = accid } });
            }
            return Json(new
            {
                code = 200,
                desc = "",
                info = new
                {
                    user.Id,
                    user.Accid,
                    user.Username,
                    user.Category,
                    user.NimUserEx.Icon,
                    Avatar = user.NimUserEx.Icon,
                    user.NimUserEx.Name,
                    NickName = user.NimUserEx.Name,
                    Nickname = user.NimUserEx.Name,
                    user.NimUserEx.Gender,
                    user.NimUserEx.Email,
                    user.NimUserEx.Mobile,
                    Birth = (user.NimUserEx.Birth == null ? "" : user.NimUserEx.Birth.Value.ToString("yyyy-MM-dd")),
                    user.NimUserEx.Country,
                    user.NimUserEx.Language,
                    user.NimUserEx.Job,
                    user.NimUserEx.Voice,
                    user.NimUserEx.About,
                    user.NimUserEx.Coins,
                    user.NimUserEx.Score
                }
            });
        }

        [HttpPost]
        public ActionResult GetByUsername(String username)
        {
            NimUser user = entities.NimUser.Single(o => o.Username == username);
            if (user == null)
            {
                return Json(new { code = 2001, desc = "没有这个人", info = new { Accid = username } });
            }
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = new
                {
                    user.Id,
                    user.Accid,
                    user.Username,
                    user.Category,
                    user.NimUserEx.Icon,
                    Avatar = user.NimUserEx.Icon,
                    user.NimUserEx.Name,
                    NickName = user.NimUserEx.Name,
                    Nickname = user.NimUserEx.Name,
                    user.NimUserEx.Gender,
                    user.NimUserEx.Email,
                    user.NimUserEx.Mobile,
                    Birth = (user.NimUserEx.Birth == null ? "" : user.NimUserEx.Birth.Value.ToString("yyyy-MM-dd")),
                    user.NimUserEx.Country,
                    user.NimUserEx.Language,
                    user.NimUserEx.Job,
                    user.NimUserEx.Voice,
                    user.NimUserEx.About,
                    user.NimUserEx.Coins,
                    user.NimUserEx.Score,
                    user.NimUserEx.School,
                    Photos = user.UploadFile.Select(o => o.Path)
                }
            });
        }

        /// <summary>
        /// 教师确认接听电话,并使状态处于接听状态下,在接听状态下的教师不会出现在排队列表中
        /// </summary>
        /// <param name="id">教师的Id</param>
        /// <param name="target">呃,暂时没用到</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Accept(Int32 id, Int32 target)
        {
            NimUser teacher = entities.NimUser.Find(id);
            teacher.IsEnable = 0;

            //添加对话记录
            entities.SaveChanges();
            return Json(new { code = 200, desc = "", info = "成功" });
        }

        /// <summary>
        /// 刷新教师在线状态,因为在查询排除列表时,刷新时间在5分钟之前的将不会列出
        /// </summary>
        /// <param name="id">要刷新的教师的Id</param>
        /// <returns>当前教师的排队名次和教师队列的前5名</returns>
        [HttpPost]
        public ActionResult Refresh(Int32? id, String accid, String username)
        {
            return Enqueue(id, accid, username, true);
        }

        public ActionResult TeacherRefreshByUserName(String username, int skip, int take)
        {
            Int64 now = DateTime.Now.Ticks;
            NimUser user = entities.NimUser.Single(o => o.Username == username);
            user.IsOnline = 1;
            user.Refresh = now;

            //默认category=1为老师  //要求是老师,在线,可用
            Expression<Func<NimUser, bool>> predicate = o => o.IsOnline == 1 && o.IsEnable == 1 && o.Category == 1 && o.Enqueue <= user.Enqueue && o.Refresh >= (user.Refresh - 3000000000L);
            var teachers = entities.NimUser.Where(predicate).OrderBy(o => o.Enqueue);
            var temp = teachers.Skip(skip).Take(take);
            return Json(new
            {
                code = 200,
                desc = "刷新成功",
                info = new
                {
                    Data = temp.Select(o => new
                    {
                        o.Id,
                        o.Accid,
                        o.NimUserEx.Name,
                        o.Username,
                        o.Category
                    }),
                    Rank = teachers.Count()
                }
            });

        }

        /// <summary>
        /// 教师入队,同时有刷新功能,入队会重新排序,但是刷新不会更改名次
        /// </summary>
        /// <param name="id">要入队的教师的Id</param>
        /// <param name="refresh">是否刷新</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enqueue(Int32? id, String accid, String username, bool refresh = false)
        {
            Int64 now = DateTime.Now.Ticks;

            NimUser user = entities.NimUser.Find(id);

            if (id != null)
            {
                user = entities.NimUser.Find(id);
            }

            if (user == null && !String.IsNullOrEmpty(accid))
            {
                user = entities.NimUser.Single(o => o.Accid == accid);
            }

            if (user == null && !String.IsNullOrEmpty(username))
            {
                user = entities.NimUser.Single(o => o.Username == username);
            }

            if (user == null)
            {
                return Json(new { code = 201, desc = (refresh ? "刷新" : "排队") + "失败", info = "用户不存在" });
            }

            user.IsOnline = 1;
            user.Refresh = now;

            //教师的刷新和入队区别是是否可用和重置入队时间
            if (!refresh)
            {
                user.IsEnable = 1;
                user.Enqueue = now;
            }
            entities.SaveChanges();

            //默认category=1为老师  //要求是老师,在线,可用
            Expression<Func<NimUser, bool>> predicate = o => o.IsOnline == 1 && o.IsEnable == 1 && o.Category == 1 && o.Enqueue <= user.Enqueue && o.Refresh >= (user.Refresh - 3000000000L);

            var teachers = entities.NimUser.Where(predicate).OrderBy(o => o.Enqueue);
            var temp = teachers.Skip(0).Take(5).ToList();
            return Json(new
            {
                code = 200,
                desc = (refresh ? "刷新" : "排队") + "成功",
                info = new
                {
                    Data = temp.Select(o => new
                    {
                        o.Id,
                        o.Accid,
                        o.Username,
                        o.NimUserEx.Name,
                        o.NimUserEx.Icon,
                        o.NimUserEx.Email,
                        o.NimUserEx.Birth,
                        o.NimUserEx.Mobile,
                        o.NimUserEx.Gender,
                        o.NimUserEx.Country,
                        o.NimUserEx.Language,
                        o.NimUserEx.Job,
                        o.NimUserEx.About,
                        o.NimUserEx.Voice
                    }),
                    Rank = teachers.Count()
                }
            });
        }

        /// <summary>
        /// 通过用户Id排队
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnqueueById(Int32 id)
        {
            Int64 now = DateTime.Now.Ticks;

            NimUser user = entities.NimUser.Find(id);
            user.IsOnline = 1;
            user.IsEnable = 1;
            user.Enqueue = now;
            user.Refresh = now;

            entities.SaveChanges();
            return Json(new
            {
                code = 200,
                desc = "",
                info = new
                {
                    user.Id,
                    user.Accid,
                    user.Username,
                    user.NimUserEx.Icon,
                    Avatar = user.NimUserEx.Icon,
                    user.NimUserEx.Name,
                    NickName = user.NimUserEx.Name,
                    Nickname = user.NimUserEx.Name,
                    user.NimUserEx.Gender,
                    user.NimUserEx.Email,
                    user.NimUserEx.Mobile,
                    Birth = (user.NimUserEx.Birth == null ? "" : user.NimUserEx.Birth.Value.ToString("yyyy-MM-dd")),
                    user.NimUserEx.Country,
                    user.NimUserEx.Language,
                    user.NimUserEx.Job,
                    user.NimUserEx.Voice,
                    user.NimUserEx.About
                }
            });
        }

        /// <summary>
        /// 通过用户Accid排队
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnqueueByAccid(String accid)
        {

            Int64 now = DateTime.Now.Ticks;

            NimUser user = entities.NimUser.Single(o => o.Accid == accid);
            user.IsOnline = 1;
            user.IsEnable = 1;
            user.Enqueue = now;
            user.Refresh = now;

            entities.SaveChanges(); return Json(new
            {
                code = 200,
                desc = "",
                info = new
                {
                    user.Id,
                    user.Accid,
                    user.Username,
                    user.NimUserEx.Icon,
                    Avatar = user.NimUserEx.Icon,
                    user.NimUserEx.Name,
                    NickName = user.NimUserEx.Name,
                    Nickname = user.NimUserEx.Name,
                    user.NimUserEx.Gender,
                    user.NimUserEx.Email,
                    user.NimUserEx.Mobile,
                    Birth = (user.NimUserEx.Birth == null ? "" : user.NimUserEx.Birth.Value.ToString("yyyy-MM-dd")),
                    user.NimUserEx.Country,
                    user.NimUserEx.Language,
                    user.NimUserEx.Job,
                    user.NimUserEx.Voice,
                    user.NimUserEx.About
                }
            });

        }

        /// <summary>
        /// 通过用户username排队
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnqueueByUsername(String username)
        {
            Int64 now = DateTime.Now.Ticks;

            NimUser user = entities.NimUser.Single(o => o.Username == username);
            user.IsOnline = 1;
            user.IsEnable = 1;
            user.Enqueue = now;
            user.Refresh = now;

            entities.SaveChanges();
            return Json(new
            {
                code = 200,
                desc = "",
                info = new
                {
                    user.Id,
                    user.Accid,
                    user.Username,
                    user.NimUserEx.Icon,
                    Avatar = user.NimUserEx.Icon,
                    user.NimUserEx.Name,
                    NickName = user.NimUserEx.Name,
                    Nickname = user.NimUserEx.Name,
                    user.NimUserEx.Gender,
                    user.NimUserEx.Email,
                    user.NimUserEx.Mobile,
                    Birth = (user.NimUserEx.Birth == null ? "" : user.NimUserEx.Birth.Value.ToString("yyyy-MM-dd")),
                    user.NimUserEx.Country,
                    user.NimUserEx.Language,
                    user.NimUserEx.Job,
                    user.NimUserEx.Voice,
                    user.NimUserEx.About
                }
            });
        }

        /// <summary>
        /// 学生获取教师,从教师队列中取第一个
        /// </summary>
        /// <param name="id">学生的用户Id</param>
        /// <returns></returns>
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
            return Json(new { code = 200, desc = "获取成功", info = new { teacher.Id, teacher.Accid, teacher.NimUserEx.Name, teacher.Username, teacher.NimUserEx.Icon } });
        }

        /// <summary>
        /// 学生选择教师
        /// </summary>
        /// <param name="id">学生的用户Id</param>
        /// <param name="target">教师的用户Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChooseTeacher(Int32 id, Int32 target)
        {
            //如果学生没有学币,那么将无法拨打电话
            Int64 now = DateTime.Now.Ticks - 3000000000L;
            NimUser student = entities.NimUser.Find(id);
            if (student == null || (student.Category != 0))
            {
                return Json(new { code = 204, desc = "没有这个学生" });
            }

            if ((student.NimUserEx.Coins ?? 0) <= 0)
            {
                student.NimUserEx.Coins = 0;
                entities.SaveChanges();
                return Json(new { code = 203, desc = "学币不足", info = new { student.NimUserEx.Id, student.NimUserEx.Name, student.NimUserEx.Coins } });
            }

            student.IsOnline = 1;
            student.Refresh = DateTime.Now.Ticks;

            NimUser teacher = entities.NimUser.Find(target);
            if (teacher == null || (teacher.Category != 1))
            {
                return Json(new { code = 202, desc = "没有这个老师" });
            }

            if (teacher.IsEnable == 0)
            {
                return Json(new { code = 201, desc = "老师已经被取走" });
            }

            teacher.IsEnable = 0;
            entities.SaveChanges();
            return Json(new { code = 200, desc = "选择成功", info = new { teacher.Id, teacher.Accid, teacher.NimUserEx.Name, teacher.Username, teacher.NimUserEx.Icon, teacher.NimUserEx.Voice } });
        }

        /// <summary>
        /// 教师的排队队列
        /// </summary>
        /// <param name="skip">跳过记录数</param>
        /// <param name="take">获取记录数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TeacherInqueue(int skip, int take)
        {
            Int64 now = DateTime.Now.Ticks;
            long refresh = now - 3000000000L;//5分钟轮循时间

            Expression<Func<NimUser, bool>> predicate = o => o.IsOnline == 1 && o.IsEnable == 1 && o.Category == 1 && (o.Enqueue < now) && (o.Refresh > refresh);//默认category=1为老师  //要求是老师,在线,可用
            Expression<Func<NimUser, long?>> keySelector = o => o.Enqueue;
            List<NimUser> teachers = entities.NimUser.Where(predicate).OrderBy(keySelector).Skip(skip).Take(take).ToList();
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = teachers.Select(o => new
                {
                    o.Id,
                    o.Accid,
                    o.NimUserEx.Name,
                    o.Username,
                    o.Category,
                    o.NimUserEx.Icon,
                    o.NimUserEx.Voice,
                    o.NimUserEx.Country,
                    o.NimUserEx.About
                })
            });
        }

        /// <summary>
        /// 取得老师,包括在线的,忙线的,掉线的
        /// </summary>
        /// <param name="skip">跳过几条</param>
        /// <param name="take">获取几条</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTeacher(int skip, int take)
        {
            try
            {//.OrderByDescending(o => o.IsOnline) //20160527 不在线的老师不用显示,即不在排队状态的老师不显示
                var teacher = entities.NimUser.Where(o => o.Category == 1 && o.IsOnline == 1).OrderByDescending(o => o.IsEnable).ThenBy(o => o.Enqueue).Skip(skip).Take(take).ToList();
                return Json(new
                {
                    code = 200,
                    desc = "查询成功",
                    info = teacher.Select(o => new
                    {
                        o.Id,
                        o.Accid,
                        Avatar = o.NimUserEx.Icon,
                        o.NimUserEx.About,
                        o.Username,
                        Nickname = o.NimUserEx.Name,
                        IsEnable = 1 == o.IsEnable,
                        IsOnline = 1 == o.IsOnline,
                        o.Enqueue,
                        o.NimUserEx.Spoken,
                        o.NimUserEx.Country,
                        Photos = o.UploadFile.Select(p => p.Path).ToList()
                    })
                });



                #region 注释代码,使用新的排队方式
                /*               
                List<NimUser> all = new List<NimUser>();

                Int64 now = DateTime.Now.Ticks;
                var teacher = entities.NimUser.Where(o => o.Category == 1).ToList();

                //在线的,包括不忙和忙的
                var online = teacher.Where(o => o.Refresh >= (now - 3000000000L)).OrderBy(o => o.Enqueue).ToList();
                //掉线的
                var offline = teacher.Where(o => o.Refresh < (now - 3000000000L)).OrderBy(o => o.Enqueue).ToList();

                //不忙的
                var free = online.Where(o => o.IsEnable == 1);
                //忙的
                var busy = online.Where(o => o.IsEnable == 0);


                //分别添加 不忙的,忙的,掉线的
                all.AddRange(free);
                all.AddRange(busy);
                all.AddRange(offline);
                var temp = all.Skip(skip).Take(take);


                return Json(new
                {
                    code = 200,
                    desc = "查询成功",
                    info = temp.Select(o => new
                    {
                        o.Id,
                        o.Accid,
                        Avatar = o.NimUserEx.Icon,
                        o.NimUserEx.About,
                        o.Username,
                        Nickname = o.NimUserEx.Name,
                        IsEnable = o.IsEnable == 1,
                        IsOnline = (now - o.Refresh) < 3000000000,
                        o.Enqueue
                    })
                });

               */
                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        /// <summary>
        /// 新的教师入队接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TeacherEnqueue(Int32? id, String accid, String username)
        {
            try
            {
                NimUser nimUser = null;
                if (id != null)
                {
                    nimUser = entities.NimUser.Find(id);
                }

                if (!String.IsNullOrEmpty(accid))
                {
                    nimUser = entities.NimUser.Single(o => o.Accid == accid);
                }

                if (!String.IsNullOrEmpty(username))
                {
                    nimUser = entities.NimUser.Single(o => o.Username == username);
                }

                if (nimUser == null)
                {
                    return Json(new { code = 201, desc = "入队失败" });
                }

                nimUser.IsEnable = 1;
                nimUser.IsOnline = 1;
                nimUser.Enqueue = DateTime.Now.Ticks;
                nimUser.Refresh = DateTime.Now.Ticks;
                entities.SaveChanges();
                return Json(new { code = 200, desc = "入队成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        /// <summary>
        /// 新的教师出队接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TeacherDequeue(Int32? id, String accid, String username)
        {
            try
            {
                NimUser nimUser = null;
                if (id != null)
                {
                    nimUser = entities.NimUser.Find(id);
                }

                if (!String.IsNullOrEmpty(accid))
                {
                    nimUser = entities.NimUser.Single(o => o.Accid == accid);
                }

                if (!String.IsNullOrEmpty(username))
                {
                    nimUser = entities.NimUser.Single(o => o.Username == username);
                }

                if (nimUser == null)
                {
                    return Json(new { code = 201, desc = "出队失败" });
                }

                nimUser.IsEnable = 0;
                nimUser.IsOnline = 0;
                nimUser.Refresh = DateTime.Now.Ticks;
                entities.SaveChanges();
                return Json(new { code = 200, desc = "出队成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        /// <summary>
        /// 新的教师刷新接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TeacherRefresh(Int32? id, String accid, String username)
        {
            try
            {
                NimUser nimUser = null;
                if (id != null)
                {
                    nimUser = entities.NimUser.Find(id);
                }

                nimUser.IsOnline = 1;
                nimUser.Refresh = DateTime.Now.Ticks;
                entities.SaveChanges();
                return Json(new { code = 200, desc = "刷新成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetPhotosByUsername(String username)
        {
            try
            {
                NimUser user = entities.NimUser.Single(o => o.Username == username);
                NimUserEx ex = user.NimUserEx;
                return Json(new
                {
                    code = 200,
                    desc = "查询成功",
                    info = new
                    {
                        user.Id,
                        user.Accid,
                        user.Username,
                        Nickname = user.NimUserEx.Name,
                        user.NimUserEx.Country,
                        ex.About,
                        ex.School,
                        ex.Spoken,
                        ex.Hobbies,
                        Photos = user.UploadFile.Select(o => o.Path)
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

    }
}
