using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ChineseChat.Library
{
    public class HttpUtil
    {
        public static String Post(String url, NameValueCollection headers, NameValueCollection parameters)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";//请求方法
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";//传递参数的格式

            //写入头文件
            foreach (String key in headers.AllKeys)
            {
                request.Headers.Add(key, headers[key]);
            }

            //写入要传递的参数
            String data = String.Join("&", parameters.AllKeys.Select(o => o + "=" + parameters[o]));
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            request.GetRequestStream().Write(buffer, 0, buffer.Length);

            //取得响应流
            StringBuilder sb = new StringBuilder();
            Stream stream = request.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            while (-1 != sr.Peek())
            {
                sb.AppendLine(sr.ReadLine());
            }
            sr.Close();

            return sb.ToString();
        }

    }
}
