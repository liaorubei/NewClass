using StudyOnline.Models;
using StudyOnline.Utils;
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
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        StudyOnlineEntities entities = new StudyOnlineEntities();
        private int pageSize = 50;

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
            CallLog chat = entities.CallLog.SingleOrDefault(o => o.ChatId == chatId);
            NimUser teacher = entities.NimUser.Find(target);

            if (chat == null)
            {
                try
                {
                    chat = new CallLog() { Coins = 0, IsBalance = 0, Duration = 0, Price = 0, BalanceS = 0, BalanceT = 0 };
                    chat.Id = Guid.NewGuid().ToString().Replace("-", "");
                    chat.ChatId = chatId;
                    chat.ChatType = chatType;
                    chat.Source = source;
                    chat.Target = target;
                    chat.Start = DateTime.Now;
                    chat.Refresh = chat.Start;

                    entities.CallLog.Add(chat);

                    //20160630创建对话的同时,把教师设置成忙状态
                    teacher.IsEnable = 0;
                    entities.SaveChanges();
                    logger.Debug(String.Format("chat history create success with:chatId={0}, chatType={1}, source={2}, target={3}", chatId, chatType, source, target));
                }
                catch (Exception ex)
                {

                    logger.Debug(String.Format("chat history create failure with:chatId={0}, chatType={1}, source={2}, target={3} and message:\r\n", chatId, chatType, source, target) + ex.Message + " \r\nStackTrace:\r\n" + ex.StackTrace + " \r\nInnerException \r\n" + ex.InnerException.StackTrace);
                    return Json(new { code = 201, desc = "创建失败", info = new { chat.Id, chat.Source, chat.Target } });
                }
            }

            return Json(new { code = 200, desc = "创建成功", info = new { chat.Id, chat.Source, chat.Target } });
        }



        /// <summary>
        /// 平衡学生的学币
        /// 要求计算的实体在上下文环境中,否则无法保存
        /// </summary>
        /// <param name="chat">在上下文环境的实体</param>
        internal static void Balance(CallLog chat)
        {
            //计算时长
            chat.Duration = (Int32)((chat.Finish - chat.Start).Value.TotalMinutes + 0.5);
            chat.Price = Constants.Price;
            chat.Coins = chat.Duration * chat.Price;
            chat.IsBalance = 1;//平衡学生学币
            chat.BalanceS = 1;//平衡学生学币
            chat.BalanceT = 0;//不用平衡教师

            //在帐号表里面减去学生相应的学币
            chat.NimUser.NimUserEx.Coins -= chat.Coins;
        }

        /// <summary>
        /// 结束语音聊天记录
        /// </summary>
        /// <param name="chatId">云信的通话ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Finish(String callId, Int64? chatId)
        {
            CallLog chat = null;

            if (!string.IsNullOrEmpty(callId))
            {
                chat = entities.CallLog.Find(callId);
            }

            if (chat == null)
            {
                chat = entities.CallLog.SingleOrDefault(o => o.ChatId == chatId);
            }

            if (chat == null)
            {
                return Json(new { code = 2001, desc = "记录为空" });
            }

            if (chat.Finish == null)
            {
                chat.Finish = DateTime.Now;
            }

            //扣费情况,一分钟一个币,如果大于等于30秒,算一分钟
            //TimeSpan s = new TimeSpan(0, 0, 0, 29,999);
            //(Int32)(s.TotalMinutes + 0.5)=0;
            //TimeSpan s = new TimeSpan(0, 0, 0, 29,1000);
            //(Int32)(s.TotalMinutes + 0.5)=1;
            var span = chat.Finish - chat.Start;
            int duration = ((Int32)(span.Value.TotalMinutes + 0.5));//29秒不算,但是如果满30秒,当一分钟算,如:5:30,按6MIN算
            int coins = duration * Constants.Price;

            //学生和老师
            NimUser student = entities.NimUser.Find(chat.Source);
            NimUser teacher = entities.NimUser.Find(chat.Target);

            ////学生扣除学币
            //if (chat.IsBalance != 1)
            //{
            //    student.NimUserEx.Coins -= coins;//从学生的帐号中去掉学币数
            //    chat.Coins = coins;//把这次的学币说写入聊天记录
            //    chat.IsBalance = 1;//平衡学币
            //    chat.Duration = duration;//这个chat的时长(单位分钟,满30秒算1分钟)
            //    chat.Price = Constants.Price;//每分钟单价
            //    chat.BalanceS = 1;//平衡学生学币
            //    chat.BalanceT = 0;//统计老师课时
            //}

            if (chat.BalanceS != 1)
            {
                Balance(chat);
            }

            //老师重新入队
            teacher.IsEnable = 1;
            teacher.IsOnline = 1;
            teacher.Enqueue = DateTime.Now;
            teacher.Refresh = DateTime.Now;

            //老师计算总课时,当月课时
            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime to = from.AddMonths(1);
            var c = entities.CallLog.Where(o => o.Target == teacher.Id && o.Start != null && o.Finish != null).Where(o => from < o.Start && o.Start <= to).Where(o => o.Id != callId);
            Int32 month = from.Month;
            Int32 d = (c.Sum(o => o.Duration) ?? 0) + duration;
            Int32 count = c.Count() + 1;

            try
            {
                entities.SaveChanges();
                return Json(new
                {
                    code = 200,
                    desc = "记录成功",
                    info = new
                    {
                        chat.Id,
                        Student = new
                        {
                            student.Id,
                            student.Username,
                            Nickname = student.NimUserEx.Name,
                            student.NimUserEx.Coins
                        },
                        Teacher = new
                        {
                            teacher.Id,
                            teacher.Username,
                            Nickname = teacher.NimUserEx.Name,
                            Summary = new { month, count, duration = d }
                        }
                        ,
                        chat.Price
                        ,
                        chat.Duration


                        //,
                        //chat.Source,
                        //chat.Target,
                        //chat.Coins
                        //,
                        //span.Value.TotalSeconds,
                        //teacher.IsEnable,
                        //teacher.IsOnline,
                        //teacher.NimUserEx.Name
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Debug(ex.StackTrace);
                return Json(new { code = 201, desc = ex.StackTrace });
            }
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
            var coins = user.Coins.Value - (((Int32)span.Value.TotalMinutes) * Constants.Price);

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
            var temp = entities.CallLog.Where(o => o.NimUser.Accid == accid && o.Start != null && o.Finish != null).OrderBy(o => o.Start).Skip(skip).Take(take).ToList();

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
                        o.NimUser1.Id,
                        o.NimUser1.NimUserEx.Name,
                        Nickname = o.NimUser1.NimUserEx.Name
                    },
                    Student = new
                    {
                        o.NimUser.Id,
                        o.NimUser.NimUserEx.Name,
                        Nickname = o.NimUser.NimUserEx.Name
                    },
                    Themes = entities.LogTheme.Where(i => i.ChatId == o.ChatId).Select(i => new { i.Theme.Name }),
                    o.Score,
                    o.Coins
                })
            });
        }

        /// <summary>
        /// 已经上传版本在用
        /// </summary>
        /// <param name="username"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
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
                        Duration = (o.Finish - o.Start).Value.ToString(@"hh\:mm\:ss"),
                        Teacher = new
                        {
                            o.NimUser1.Id,
                            Nickname = o.NimUser1.NimUserEx.Name,
                            o.NimUser1.Username
                        },
                        Student = new
                        {
                            o.NimUser.Id,
                            Nickname = o.NimUser.NimUserEx.Name,
                            o.NimUser.Username
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
                            o.NimUser1.Id,
                            Nickname = o.NimUser1.NimUserEx.Name,
                            o.NimUser1.Username
                        },
                        Student = new
                        {
                            o.NimUser.Id,
                            Nickname = o.NimUser.NimUserEx.Name,
                            o.NimUser.Username
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
        /// 获取学习记录接口
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="type">0为学生,1为老师</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetByUsername(String username, Int32 type, Int32 skip, Int32 take)
        {
            NimUser user = entities.NimUser.Single(o => o.Username == username);
            Expression<Func<CallLog, bool>> predicate = o => (type == 0 ? o.Source == user.Id : o.Target == user.Id);
            var temp = entities.CallLog.Where(o => o.Start != null && o.Finish != null).Where(predicate).OrderByDescending(o => o.Start).Skip(skip).Take(take).ToList();
            temp.Select(o => o.Coins).Sum();

            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    Start = o.Start.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    Finish = o.Finish.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    (o.Finish - o.Start).Value.TotalSeconds,
                    Duration = (Int32)((o.Finish - o.Start).Value.TotalMinutes + 0.5),
                    Teacher = new
                    {
                        o.NimUser1.Id,
                        Nickname = o.NimUser1.NimUserEx.Name ?? "",
                        o.NimUser1.Username
                    },
                    Student = new
                    {
                        o.NimUser.Id,
                        Nickname = o.NimUser.NimUserEx.Name ?? "",
                        o.NimUser.Username
                    },
                    Themes = entities.LogTheme.Where(i => i.ChatId == o.ChatId).Select(i => new { i.Theme.Name }),
                    o.Score,
                    o.Coins
                })
            });
        }

        /// <summary>
        /// 取得通话记录
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="skip">跳过</param>
        /// <param name="take">获取</param>
        /// <param name="from">从某时 格式为 yyyy-MM-dd HH:mm:ss</param>
        /// <param name="to">到某时 yyyy-MM-dd HH:mm:ss</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetByUserId(Int32? id, Int32? skip, Int32? take, DateTime? from, DateTime? to)
        {
            NimUser user = entities.NimUser.Find(id);
            if (user == null)
            {
                return Json(new { code = 201, desc = "指定用户名不存在" });
            }

            Expression<Func<CallLog, bool>> userPredicate = o => user.Category == 0 ? o.Source == id : o.Target == id;
            Expression<Func<CallLog, bool>> rangePredicate = o => from < o.Start && o.Start <= to;
            List<CallLog> chats = entities.CallLog.Where(o => o.Start != null && o.Finish != null).Where(userPredicate).Where(rangePredicate).OrderByDescending(o => o.Start).Skip(skip ?? 0).Take(take ?? pageSize).ToList();

            var tha = chats.Where(o => o.Duration == null);
            if (tha.Any())
            {
                foreach (var item in tha)
                {
                    item.Duration = (Int32)((item.Finish - item.Start).Value.TotalMinutes + 0.5);
                }
                try
                {
                    entities.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }


            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = chats.Select(o => new
                {
                    Start = o.Start.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    Finish = o.Finish.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    Duration = (Int32)((o.Finish - o.Start).Value.TotalMinutes + 0.5),
                    Teacher = new
                    {
                        o.NimUser1.Id,
                        Nickname = o.NimUser1.NimUserEx.Name ?? "",
                        o.NimUser1.Username
                    },
                    Student = new
                    {
                        o.NimUser.Id,
                        Nickname = o.NimUser.NimUserEx.Name ?? "",
                        o.NimUser.Username
                    },
                    Themes = entities.LogTheme.Where(i => i.ChatId == o.ChatId).Select(i => new { i.Theme.Name }),
                    o.Score,
                    o.Coins
                })
            });
        }

        [HttpPost]
        public ActionResult GetByUserIdAndYearMonth(Int32? id, Int32? skip, Int32? take, DateTime? from, DateTime? to)
        {
            NimUser user = entities.NimUser.Find(id);
            if (user == null)
            {
                return Json(new { code = 201, desc = "指定用户名不存在" });
            }

            Expression<Func<CallLog, bool>> userPredicate = o => user.Category == 0 ? o.Source == id : o.Target == id;
            Expression<Func<CallLog, bool>> rangePredicate = o => from < o.Start && o.Start <= to;
            List<CallLog> chats = entities.CallLog.Where(o => o.Start != null && o.Finish != null).Where(userPredicate).Where(rangePredicate).OrderByDescending(o => o.Start).Skip(skip ?? 0).Take(take ?? pageSize).ToList();

            var tha = chats.Where(o => o.Duration == null);
            if (tha.Any())
            {
                foreach (var item in tha)
                {
                    item.Duration = (Int32)((item.Finish - item.Start).Value.TotalMinutes + 0.5);
                }
                try
                {
                    entities.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }


            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = chats.Select(o => new
                {
                    Start = o.Start.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    Finish = o.Finish.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    Duration = (Int32)((o.Finish - o.Start).Value.TotalMinutes + 0.5),
                    Teacher = new
                    {
                        o.NimUser1.Id,
                        Nickname = o.NimUser1.NimUserEx.Name ?? "",
                        o.NimUser1.Username
                    },
                    Student = new
                    {
                        o.NimUser.Id,
                        Nickname = o.NimUser.NimUserEx.Name ?? "",
                        o.NimUser.Username
                    },
                    Themes = entities.LogTheme.Where(i => i.ChatId == o.ChatId).Select(i => new { i.Theme.Name }),
                    o.Score,
                    o.Coins
                })
            });
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


    }
}
