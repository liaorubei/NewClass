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
            CallLog model = entities.CallLog.FirstOrDefault(o => o.ChatId == chatId);

            if (model == null)
            {
                return Json(new { code = 2001, desc = "目标记录没有找到", info = new { model.Id, model.Source, model.Target, model.Start.Value.Ticks, model.Finish } });
            }
            model.Finish = DateTime.Now;
            entities.SaveChanges();
            return Json(new { code = 200, desc = "", info = new { model.Id, model.Source, model.Target, model.Start.Value.Ticks, model.Finish } });
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
                    Teacher = new { o.Teacher.Id, o.Teacher.NimUserEx.Name },
                    Student = new { o.Student.Id, o.Student.NimUserEx.Name },
                    Themes = entities.LogTheme.Where(i => i.ChatId == o.ChatId).Select(i => new { i.Theme.Name }),
                    o.Score
                })
            });
        }

        //public ActionResult GetCallLog(String accid, Int32 category, Int32 skip, Int32 take)
        //{
        //    Expression<Func<CallLog, bool>> predicate = o=> category==0?o=>;

        //    entities.CallLog.Where(o => o.Start != null && o.Finish != null).Where(predicate);
        //    return null;
        //}

        /// <summary>
        /// 给指定目标的通话ID添加对话主题
        /// </summary>
        /// <param name="chatId">通话ID</param>
        /// <param name="themeId">主题Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddTheme(Int64 chatId, int themeId)
        {
            LogTheme model = new LogTheme();
            model.ChatId = chatId;
            model.ThemeId = themeId;
            entities.LogTheme.Add(model);
            entities.SaveChanges();
            return Json(new { code = 200, desc = "", info = new { model.ChatId, model.ThemeId } });
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
