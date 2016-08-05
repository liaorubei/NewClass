using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using StudyOnline.Models;
using StudyOnline.Areas.Api.Controllers;

namespace StudyOnline.Quartz
{
    /// <summary>
    /// 通话记录同步定时执行类
    /// 因为在实时语音通话时,会有不正常退出情况,这样一来每个通话记录就没有检测到结束标识了,没有结束标识就没法平衡学币,因此要定时查看并平衡学币,避免出现通话记录丢失情况
    /// </summary>
    public class ChatJob : IJob
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Execute(IJobExecutionContext context)
        {
            log.Info("----------now sync calllog----------");
            StudyOnlineEntities entities = new StudyOnlineEntities();

            //过滤条件要求是在当前时间8个小时之前的数据才处理,防止和现在的正在进行的通话冲突处理,因为如果此时正有一个通话,那么他的学生平衡就是不为1的
            DateTime deadlineTime = DateTime.Now.AddHours(-8);
            List<CallLog> chats = entities.CallLog.OrderByDescending(o => o.Start).Where(o => o.Start < deadlineTime && o.Finish == null && o.Refresh != null).ToList();
            log.Info("deadlineTime" + deadlineTime.ToString("yyyy-MM-dd HH:mm:ss"));

            foreach (var item in chats)
            {
                //记录下这记录的原始开始时间和结束时间
                log.Info(item.Id + " start:" + item.Start.Value.ToString("yyyy-MM-dd HH:mm:ss") + " refersh:" + item.Refresh.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                item.Finish = item.Refresh;//当结束标记为空的时候,用最后的刷新时间代替
                CallLogController.Balance(item);
            }
            entities.SaveChanges();
            entities.Dispose();
        }

    }
}