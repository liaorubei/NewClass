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
            Int32? i = null;
            Int32? k = null;
            i = (i ?? 4) + (k ?? 5);
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}
