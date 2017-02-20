using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Models
{
    public class Constant
    {
    }

    public enum Client : Int32
    {
        安卓学生端 = 0, 安卓教师端 = 1, 苹果学生端 = 2, 苹果教师端 = 3
    }

    public enum ChatType:Int32
    {
        通话开始 = 0,
        通话结束 = 1
    }
}