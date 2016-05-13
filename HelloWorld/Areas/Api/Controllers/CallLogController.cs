using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class CallLogController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();

        /// <summary>
        /// 开始添加一条聊天记录
        /// </summary>
        /// <param name="chatId">云信的通话ID</param>
        /// <param name="chatType">通话类型</param>
        /// <param name="source">发起人</param>
        /// <param name="target">目标人</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Start(Int64 chatId, Int32 chatType, Int32 source, Int32 target)
        {
            CallLog log = new CallLog();
            log.Id = Guid.NewGuid().ToString().Replace("-", "");
            log.ChatId = chatId;
            log.ChatType = chatType;
            log.Source = source;
            log.Target = target;
            log.Start = DateTime.Now;
            log.Coins = 0;//必须写上,不然为空不好计算

            entities.CallLog.Add(log);
            entities.SaveChanges();
            return Json(new { code = 200, desc = "", info = new { log.Id, log.Source, log.Target } });
        }

        /// <summary>
        /// 结束语音聊天记录
        /// </summary>
        /// <param name="chatId">云信的通话ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Finish(Int64 chatId)
        {
            CallLog chat = entities.CallLog.FirstOrDefault(o => o.ChatId == chatId);
            if (chat == null)
            {
                return Json(new { code = 2001, desc = "挂断失败" });
            }

            if (chat.Finish == null)
            {
                chat.Finish = DateTime.Now;
            }

            //扣费情况,一分钟一个币
            var span = chat.Finish - chat.Start;
            int coin = Convert.ToInt32(span.Value.TotalMinutes);//29秒不算,但是如果满30秒,当一秒算,如:5:30,按6MIN算

            //如果大于30秒,算一分钟

            //如果不够,则补够
            NimUserEx user = entities.NimUserEx.Find(chat.Source);

            //老师重新入队
            NimUser teacher = entities.NimUser.Find(chat.Target);
            teacher.IsEnable = 1;
            teacher.IsOnline = 1;
            teacher.Enqueue = DateTime.Now.Ticks;
            teacher.Refresh = DateTime.Now.Ticks;

            user.Coins -= coin;//从学生的帐号中去掉学币数
            chat.Coins = coin;//把这次的浑身说写入聊天

            entities.SaveChanges();
            return Json(new
            {
                code = 200,
                desc = "挂断成功",
                info = new
                {
                    chat.Id,
                    Student = new
                    {
                        chat.Student.NimUserEx.Id,
                        Nickname = chat.Student.NimUserEx.Name,
                        chat.Student.NimUserEx.Coins
                    },
                    chat.Source,
                    chat.Target,
                    chat.Coins,
                    span.Value.TotalSeconds
                }
            });
        }

        /// <summary>
        /// 刷新通话记录,按要求每分钟刷新一次
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Refresh(String callId, Int64? chatId)
        {
            CallLog call = entities.CallLog.Find(callId);
            call.Refresh = DateTime.Now;
            NimUserEx user = entities.NimUserEx.Find(call.Source);
            entities.SaveChanges();

            var span = call.Refresh - call.Start;
            var coins = (user.Coins.Value - (Int32)span.Value.TotalMinutes);

            if (coins <= 0)
            {
                return Json(new
                {
                    code = 201,
                    desc = "学币不足",
                    info = new
                    {
                        user.Id,
                        user.Name,
                        Nickname = user.Name,
                        Coins = coins
                    }
                });
            }
            return Json(new
            {
                code = 200,
                desc = "刷新成功",
                info = new
                {
                    user.Id,
                    user.Name,
                    Nickname = user.Name,
                    Coins = coins
                }
            });
        }

        /// <summary>
        /// 通过学生的Id获取学生的聊天记录
        /// </summary>
        /// <param name="id">学生的帐号Id</param>
        /// <param name="skip">跳过记录数</param>
        /// <param name="take">获取记录数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStudentCalllogById(int id, Int32 skip, Int32 take)
        {
            var temp = entities.CallLog.Where(o => o.Source == id).OrderBy(o => o.Start).Skip(skip).Take(take);

            return Json(new
            {
                code = 200,
                desc = "",
                info = temp.Select(o => new
                {
                    o.Target,
                    o.Start,
                    o.Finish
                })
            });
        }

        /// <summary>
        /// 通过学生的Accid获取学生的聊天记录
        /// </summary>
        /// <param name="accid">学生的帐号Accid</param>
        /// <param name="skip">路过记录数</param>
        /// <param name="take">获取记录数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStudentCalllogByAccId(String accid, Int32 skip, Int32 take)
        {
            var temp = entities.CallLog.Where(o => o.Student.Accid == accid && o.Start != null && o.Finish != null).OrderBy(o => o.Start).Skip(skip).Take(take).ToList();

            return Json(new
            {
                code = 200,
                desc = "",
                info = temp.Select(o => new
                {
                    o.Target,
                    Start = (o.Start == null ? null : o.Start.Value.ToString("yyyy-MM-dd HH:mm:ss")),
                    Finish = (o.Finish == null ? null : o.Finish.Value.ToString("yyyy-MM-dd HH:mm:ss")),
                    (o.Finish - o.Start).Value.TotalSeconds,
                    Teacher = new
                    {
                        o.Teacher.Id,
                        o.Teacher.NimUserEx.Name,
                        Nickname = o.Teacher.NimUserEx.Name
                    },
                    Student = new
                    {
                        o.Student.Id,
                        o.Student.NimUserEx.Name,
                        Nickname = o.Student.NimUserEx.Name
                    },
                    Themes = entities.LogTheme.Where(i => i.ChatId == o.ChatId).Select(i => new { i.Theme.Name }),
                    o.Score,
                    o.Coins
                })
            });
        }

        [HttpPost]
        public ActionResult GetStudentByUsername(String username, Int32 skip, Int32 take)
        {
            try
            {
                NimUser nimuser = entities.NimUser.Single(o => o.Username == username);
                var temp = entities.CallLog.Where(o => o.Source == nimuser.Id && o.Start != null && o.Finish != null).OrderByDescending(o => o.Start).Skip(skip).Take(take).ToList();
                return Json(new
                {
                    code = 200,
                    desc = "查询成功",
                    info = temp.Select(o => new
                    {
                        o.Target,
                        Start = o.Start.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        Finish = o.Finish.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        (o.Finish - o.Start).Value.TotalSeconds,
                        Teacher = new
                        {
                            o.Teacher.Id,
                            Nickname = o.Teacher.NimUserEx.Name,
                            o.Teacher.Username
                        },
                        Student = new
                        {
                            o.Student.Id,
                            Nickname = o.Student.NimUserEx.Name,
                            o.Student.Username
                        },
                        Themes = entities.LogTheme.Where(i => i.ChatId == o.ChatId).Select(i => new { i.Theme.Name }),
                        o.Score,
                        o.Coins
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTeacherByUsername(String username, Int32 skip, Int32 take)
        {
            try
            {
                NimUser nimuser = entities.NimUser.Single(o => o.Username == username);
                var temp = entities.CallLog.Where(o => o.Target == nimuser.Id && o.Start != null && o.Finish != null).OrderByDescending(o => o.Start).Skip(skip).Take(take).ToList();
                return Json(new
                {
                    code = 200,
                    desc = "查询成功",
                    info = temp.Select(o => new
                    {
                        Start = o.Start.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        Finish = o.Finish.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        (o.Finish - o.Start).Value.TotalSeconds,
                        Teacher = new
                        {
                            o.Teacher.Id,
                            Nickname = o.Teacher.NimUserEx.Name,
                            o.Teacher.Username
                        },
                        Student = new
                        {
                            o.Student.Id,
                            Nickname = o.Student.NimUserEx.Name,
                            o.Student.Username
                        },
                        Themes = entities.LogTheme.Where(i => i.ChatId == o.ChatId).Select(i => new { i.Theme.Name }),
                        o.Score,
                        o.Coins
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        /// <summary>
        /// 给指定目标的通话ID添加对话主题
        /// </summary>
        /// <param name="chatId">通话ID</param>
        /// <param name="themeId">主题Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddTheme(Int64 chatId, int themeId)
        {
            try
            {
                LogTheme model = new LogTheme();
                model.ChatId = chatId;
                model.ThemeId = themeId;
                entities.LogTheme.Add(model);
                entities.SaveChanges();
                return Json(new { code = 200, desc = "添加成功", info = new { model.ChatId, model.ThemeId } });
            }
            catch (Exception ex)
            {
                return Json(new { code = 201, desc = ex.Message });
            }
        }

        /// <summary>
        /// 给指定通话Id评分
        /// </summary>
        /// <param name="chatId">通话ID</param>
        /// <param name="score">评分值,共5分</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Rating(Int64 chatId, int score)
        {
            CallLog log = entities.CallLog.FirstOrDefault(o => o.ChatId == chatId);
            if (log == null)
            {
                return Json(new { code = 2001, desc = "目标聊天记录不存在", info = new { log.ChatId, log.ChatType, log.Score } });
            }

            log.Score = score;
            entities.SaveChanges();

            return Json(new { code = 200, desc = "评分成功", info = new { log.ChatId, log.ChatType, log.Score } });
        }

        // POST: Api/CallLog/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
