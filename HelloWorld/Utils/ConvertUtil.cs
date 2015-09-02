using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Utils
{
    public class ConvertUtil
    {
        internal static int ToInt32(string p1, int p2)
        {
            int result = 0;
            if (Int32.TryParse(p1, out result))
            {
                return result;
            };
            return p2;
        }
    }
}