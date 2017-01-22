using Quartz;
using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Quartz
{
    /// <summary>
    /// 教师定时刷新任务,如果5分钟中没有收到教师的刷新刷新请求,那么就把教师下线
    /// </summary>
    public class TeacherRefreshJob : IJob
    {
        log4net.ILog log = log4net.LogManager.GetLogger("TeacherRefreshJob");
        public void Execute(IJobExecutionContext context)
        {
            //log.Info("现在异步处理教师在线情况");
            StudyOnlineEntities entities = new StudyOnlineEntities();
            //只刷新安卓端,手机系统,安卓为1,苹果为2,其它为0
            var a = DateTime.Now.AddMinutes(-5);
            //Console.WriteLine(a.ToString("yyyy-MM-dd HH:mm:ss"));
            var teacher = entities.NimUser.Where(o => o.Category == 1 && o.IsOnline == 1 && o.IsEnable == 1 && o.Refresh < a && o.System == 1).ToList();
            foreach (var item in teacher)
            {
                log.Debug("让教师下线：" + item.Username + " 最后刷新时间：" + item.Refresh.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                //Console.WriteLine(item.Username + "  " + item.Refresh.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                item.IsOnline = 0;
                item.IsQueue = 0;
                item.IsEnable = 0;
            }

            entities.SaveChanges();
            entities.Dispose();
        }
    }
}