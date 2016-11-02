using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Utils
{
    public class ConvertUtil
    {
        /// <summary>
        /// 将一个字符串转化成Int32类型数据,如果转化失败,返回默认值
        /// </summary>
        /// <param name="value">要转化的字符串</param>
        /// <param name="defaultValue">转化失败返回的默认值</param>
        /// <returns></returns>
        internal static int ToInt32(string value, int defaultValue)
        {
            int result = 0;
            if (Int32.TryParse(value, out result))
            {
                return result;
            };
            return defaultValue;
        }
    }
}