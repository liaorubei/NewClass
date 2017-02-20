using Quartz;
using Quartz.Impl;
using StudyOnline.Quartz;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace StudyOnline
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private IScheduler scheduler;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            //web api 路由注册
            //GlobalConfiguration.Configure(WebApiConfig.Register);


            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));

            // Grab the Scheduler instance from the Factory 
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            IJobDetail jobDetail = JobBuilder.Create<ChatJob>().WithIdentity("job1").Build();

            //0 0 12 * * ?    Fire at 12pm(noon) every day
            //0 15 10 ? * *   Fire at 10:15am every day
            //0 15 10 * * ?   Fire at 10:15am every day
            //0 15 10 * * ? * Fire at 10:15am every day
            ITrigger trigger = TriggerBuilder.Create().WithIdentity("trigger1").StartNow().WithCronSchedule("00 30 * * * ?").Build();
            scheduler.ScheduleJob(jobDetail, trigger);

            IJobDetail jobDetail2 = JobBuilder.Create<TeacherAuditJob>().WithIdentity("jobDetail2").Build();
            ITrigger trigger2 = TriggerBuilder.Create().WithIdentity("trigger2").StartNow().WithCronSchedule("00 30 * * * ?").Build();
            scheduler.ScheduleJob(jobDetail2, trigger2);

            IJobDetail jobDetail3 = JobBuilder.Create<TeacherRefreshJob>().WithIdentity("jobDetail3").Build();
            ITrigger trigger3 = TriggerBuilder.Create().WithIdentity("trigger3").StartNow().WithSimpleSchedule(o => o.WithIntervalInMinutes(1).RepeatForever()).Build();
            scheduler.ScheduleJob(jobDetail3, trigger3);

            //and start it off
            scheduler.Start();
        }

        protected void Application_End()
        {
            scheduler.Shutdown();
        }


    }
}