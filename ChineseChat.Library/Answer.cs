using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChat.Library
{
    public class Answer 
    {
        public Int32 code { get; set; }
        public String desc { get; set; }
        public Info info { get; set; }
    }

    public class Info
    {
        public String accid { get; set; }
        public String token { get; set; }
        public String name { get; set; }
    }
}
