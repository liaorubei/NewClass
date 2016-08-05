using System;
using System.Collections.Generic;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {

            List<int> a = new List<int>();
            a.ForEach(o=>o.ToString());

            Console.Read();
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
