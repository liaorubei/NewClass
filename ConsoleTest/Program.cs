using ChineseChat.Library;
using Newtonsoft.Json;
using StudyOnline.Models;
using StudyOnline.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace ConsoleTest
{
    // using System.Threading;

    class Account
    {
        private Object thisLock = new Object();
        int balance;

        Random r = new Random();

        public Account(int initial)
        {
            balance = initial;
        }

        int Withdraw(int amount)
        {

            // This condition never is true unless the lock statement
            // is commented out.
            if (balance < 0)
            {
                throw new Exception("Negative Balance");
            }

            // Comment out the next line to see the effect of leaving out 
            // the lock keyword.
            lock (thisLock)
            {
                Console.WriteLine("Name=" + Thread.CurrentThread.Name + "_top");
                if (balance >= amount)
                {
                    Console.WriteLine("Name=" + Thread.CurrentThread.Name + " Balance before Withdrawal :  " + balance);
                    Console.WriteLine("Name=" + Thread.CurrentThread.Name + "Amount to Withdraw        : -" + amount);
                    balance = balance - amount;
                    Console.WriteLine("Name=" + Thread.CurrentThread.Name + "Balance after Withdrawal  :  " + balance);
                    Console.WriteLine("Name=" + Thread.CurrentThread.Name + "_end");
                    return amount;
                }
                else
                {
                    Console.WriteLine("Name=" + Thread.CurrentThread.Name + "_end");
                    return 0; // transaction rejected
                }
            }
        }

        public void DoTransactions()
        {
            for (int i = 0; i < 100; i++)
            {
                Withdraw(r.Next(1, 100));
            }
        }
    }

    class Program
    {

        static void Main(string[] args)
        {

            Thread[] threads = new Thread[10];
            Account acc = new Account(1000);
            for (int i = 0; i < 9; i++)
            {
                Thread t = new Thread(new ThreadStart(acc.DoTransactions));
                t.Name = "__" + i + "__";
                threads[i] = t;
            }
            for (int i = 0; i < 9; i++)
            {
                threads[i].Start();
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
