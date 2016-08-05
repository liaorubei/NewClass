using ChineseChat.Library;
using Newtonsoft.Json;
using Quartz;
using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Quartz
{
    /// <summary>
    /// 教师审核同步数据类,根据表[Teacherreginfo]的审核记录,定时把IsEnable=1的记录同步到NimUser表中,并关联云信IM帐号体系
    /// </summary>
    public class TeacherAuditJob : IJob
    {
        log4net.ILog log = log4net.LogManager.GetLogger("TeacherAuditJob");
        public void Execute(IJobExecutionContext context)
        {
            log.Info("now sync teacher");
            try
            {
                StudyOnlineEntities entities = new StudyOnlineEntities();
                List<Teacherreginfo> teachers = entities.Teacherreginfo.Where(o => o.IsEnable == 1 && o.IsSync !=1).ToList();
                foreach (var item in teachers)
                {
                    //先创建云信帐号
                    //同步云信帐号系统
                    String json = NimUtil.UserCreate(Guid.NewGuid().ToString().Replace("-", ""), null, null, HttpUtility.UrlEncode(item.Truename));
                    Answer a = JsonConvert.DeserializeObject<Answer>(json);
                    if (a.code == 200)
                    {
                        //修改数据库状态
                        item.IsSync = 1;

                        //创建新用户
                        NimUser nimUser = new NimUser() { Username = item.Username, Password = item.Password, Category = 1, Accid = a.info.accid, Token = a.info.token, CreateDate = DateTime.Now, IsActive = 1, IsOnline = 0 };
                        nimUser.NimUserEx = new NimUserEx() { Email = item.Username, Mobile = item.Phonenumber, Spoken = item.ForeignLanguages, School = item.Education, About = item.Note };
                        entities.NimUser.Add(nimUser);
                    }
                    log.Info(String.Format("Syncint the teacher:{0}", item.Username));
                }

                entities.SaveChanges();
            }
            catch (Exception)
            {
                log.Info("sync failure");
            }

        }
    }
}