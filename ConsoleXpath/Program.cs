using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Kfstorm.LrcParser;
using NAudio.Wave;

namespace ConsoleXpath
{


    /// <summary>
    /// 本程序意在用来爬取散文,主要API为HtmlAgilityPack,HtmlWeb,HtmlDocument
    /// </summary>
    class Program
    {


        static void Main(string[] args)
        {
            FileInfo info = new FileInfo("");
            //      info.Length;

            Mp3FileReader reader = new Mp3FileReader(@"D:\TTPmusic\huanzi.mp3");
            TimeSpan duration = reader.TotalTime;

            Console.WriteLine("{0}:{1}", duration.Minutes, duration.Seconds);
            Console.WriteLine("TotalMilliseconds=" + duration.TotalMilliseconds);
            Console.ReadLine();

        }

        private static void newParse()
        {
            StreamReader reader = new StreamReader("d:\\12306.lrc");
            string lrcstring = reader.ReadToEnd();
            //ILrcFile 

            ILrcFile lrcFile = LrcFile.FromText(lrcstring);
            var ii = LrcFile.FromText(lrcstring);
            foreach (var item in ii.Lyrics)
            {
                Console.WriteLine(item.Timestamp + "  " + item.Content);
            }
        }

        /// <summary>
        /// 歌词处理函数,测试方法
        /// </summary>
        private static void LrcParse()
        {
            Dictionary<TimeSpan, String> dict = new Dictionary<TimeSpan, string>();

            StreamReader reader = new StreamReader("D:\\新建文本文档.txt", Encoding.Default);
            int index = 0;
            String lrcString = reader.ReadToEnd().Replace("\n", "");
            int startIndex = -1;
            int nextIndex = -1;
            do
            {
                startIndex = lrcString.IndexOf('[', index);
                index++;
                nextIndex = lrcString.IndexOf('[', index);

                //如果已经到结尾,则把最后的索引设置为长度,因为结尾已经不可能还有'['字符了
                if (nextIndex == -1)
                {
                    nextIndex = lrcString.Length;
                }


                //如果前面的字符是']',表明这歌词共用多个时间标签,所以把这些时间标签放在一起
                while (lrcString.Substring(nextIndex - 1, 1) == "]")
                {
                    nextIndex = lrcString.IndexOf('[', nextIndex + 1);
                    if (nextIndex == -1)
                    {
                        nextIndex = lrcString.Length;
                    }
                }

                String result = lrcString.Substring(startIndex, (nextIndex - startIndex));
                var ddd = ParseTime(result.Trim());
                if (ddd != null)
                {
                    dict.Add(ddd.key, ddd.value);
                }
                index = nextIndex;
            } while (index < lrcString.Length);
            reader.Close();

            var d = dict.OrderBy(m => m.Key.TotalMilliseconds);
            foreach (var item in d)
            {
                Console.WriteLine("{0}={1}", item.Key.ToString(@"hh\:mm\:ss"), item.Value);
            }

        }

        private static Model ParseTime(string lrc)
        {
            Regex regex = new Regex(@"\[([0-9.:]*)\]+(.*)", RegexOptions.Compiled);

            if (regex.IsMatch(lrc))
            {

                var mc = regex.Split(lrc);
                Model m = new Model();
                m.key = toTimeSpan(mc[1]);
                m.value = mc[2];

                return m;
            }
            return null;
        }

        private static TimeSpan toTimeSpan(string p)
        {
            int s冒号 = p.IndexOf(':');
            int s逗号 = p.IndexOf('.');
            int min = Convert.ToInt32(p.Substring(0, s冒号)); ;
            int sec = Convert.ToInt32(p.Substring(s冒号 + 1, s逗号 - s冒号 - 1));
            int m = Convert.ToInt32(p.Substring(s逗号 + 1, p.Length - s逗号 - 1));
            return new TimeSpan(0, 0, min, sec, m);
        }

        private static void XmlParse()
        {
            Random random = new Random();
            StudyOnlineEntities entities = new StudyOnlineEntities();
            List<String> urls = new List<string>();

            List<Document> documents = new List<Document>();

            HtmlWeb web = new HtmlWeb();

            for (int i = 1; i < 11; i++)
            {
                var doc = web.Load("http://www.sanwen.net/sanwen/?p=" + i);
                var nodes = doc.DocumentNode.SelectNodes("//div[@class='categorylist']/ul/li/h3/a[2]");
                foreach (var item in nodes)
                {
                    string url = "http://www.sanwen.net" + item.GetAttributeValue("href", "string");
                    Console.WriteLine("标题={0} 地址={1}", item.InnerHtml, url);
                    Console.WriteLine("=========");
                    documents.Add(new Document() { Title = item.InnerText, SoundPath = url, AddDate = DateTime.Now });

                }
            }

            HtmlWeb contentWeb = new HtmlWeb();
            foreach (var item in documents)
            {
                var doc = contentWeb.Load(item.SoundPath);

                //清除所有的脚本,样式,注释,因为会影响在富文本编辑器中的显示
                foreach (var script in doc.DocumentNode.Descendants("script").ToArray())
                    script.Remove();
                foreach (var style in doc.DocumentNode.Descendants("style").ToArray())
                    style.Remove();
                foreach (var comment in doc.DocumentNode.SelectNodes("//comment()").ToArray())
                    comment.Remove();//新增的代码

                var node = doc.DocumentNode.SelectSingleNode("//div[@class='content']");

                Console.WriteLine();
                Console.WriteLine(node.InnerHtml);
                item.LevelId = random.Next(4) + 1;
                item.Contents = node.InnerHtml;

                entities.Document.Add(item);

            }

            //保存到数据库
            entities.SaveChanges();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("抓取完毕");
        }
    }

    class Model
    {
        public TimeSpan key;
        public String value;
    }
}
