using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan span = new TimeSpan(0, 5, 29);
            Console.WriteLine("TotalMinutes=" + Convert.ToInt32(span.TotalMinutes));
            Console.ReadLine();
        }
    }
}
