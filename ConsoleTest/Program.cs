using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //NewMethod1();
            List<String> s = new List<string> { "Teacher00", "Teacher00", "Teacher00", "Teacher00", "Teacher00", "Teacher00", "Teacher00" };

            foreach (var item in s.Where(o => o== "Teacher00"))
            {
                Console.WriteLine(item);
            }

            Console.Read();
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
