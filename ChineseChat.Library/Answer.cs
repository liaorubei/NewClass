using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChat.Library
{
    public class Answer
    {
        public String desc { get; set; }
        public String code { get; set; }
        public Info info { get; set; }
    }

    public class Info
    {
        public String accid { get; set; }
        public String token { get; set; }
        public String name { get; set; }
    }
}
