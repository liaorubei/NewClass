using ChineseChat.Library;
using Newtonsoft.Json;
using StudyOnline.Models;
using StudyOnline.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            NimUserEx ex = new NimUserEx();
            ex.Coins = ex.Coins + 0;
            if ((ex.Coins ?? 0) <= 0)
            {
                Console.WriteLine("ds");
            }

            Console.Read();
        }

        private static void NewMethod3()
        {
            StudyOnlineEntities entities = new StudyOnlineEntities();
            DateTime now = DateTime.Now;
            for (int i = 1; i <= 150; i++)
            {
                //创建帐号
                NimUser nimuser = new NimUser() { Accid = Guid.NewGuid().ToString().Replace("-", ""), Category = 0, IsActive = 1, IsEnable = 1, Username = String.Format("CFFD{0:D3}", i), Password = EncryptionUtil.Md5Encode("00000000"), CreateDate = now, NimUserEx = new NimUserEx() { Coins = 600 } };

                //同步云信
                String json = NimUtil.UserCreate(nimuser.Accid, null, null, null);
                Answer a = JsonConvert.DeserializeObject<Answer>(json);
                if (a.code == 200)
                {
                    nimuser.Token = a.info.token;
                }

                //保存数据
                entities.NimUser.Add(nimuser);
                Console.WriteLine("帐号:{0},密码:{2},学币:{1},accid={3},token={4}", nimuser.Username, nimuser.NimUserEx.Coins, nimuser.Password, nimuser.Accid, nimuser.Token);
            }

            entities.SaveChanges();
        }

        private static void NewMethod2()
        {
            var a = new DateTime(DateTime.Now.Year, 2, 1);
            var b = a.AddMonths(1);
            Console.WriteLine(a.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(b.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private static void NewMethod1()
        {
            var t = DateTime.Now;
            Console.WriteLine(t.ToString("yyyyMMdd HH:mm:ss.fffff"));

            TimeSpan s = new TimeSpan(0, 0, 59, 29, 1000);
            Console.WriteLine(s.ToString(@"mm\:ss"));
            Console.WriteLine((Int32)(s.TotalMinutes + 0.5));
        }
    }
}
