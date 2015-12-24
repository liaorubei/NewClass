using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;



using System.Text.RegularExpressions;
using Kfstorm.LrcParser;
using NAudio.Wave;
using NAudio.FileFormats.Mp3;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;
using ChineseChat.Library;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace ConsoleXpath
{

    class Program
    {

        //参数 参数说明
        //AppKey 开发者平台分配的appkey
        //Nonce 随机数（最大长度128个字符）
        //CurTime 当前UTC时间戳，从1970年1月1日0点0 分0 秒开始到现在的秒数(String)
        //CheckSum SHA1(AppSecret + Nonce + CurTime),三个参数拼接的字符串，进行SHA1哈希计算，转化成16进制字符(String，小写)
        //App Key:db75c3901c1a2029d0dd668975b580e0
        //App Secret:8b928c19e4cc




        //db75c3901c1a2029d0dd668975b580e0
        //HttpWebRequest
        static void Main(string[] args)
        {

            testusercreate();
           // UserUpdate();
           // SelectMethod();
            Console.ReadLine();
        }

        private static void UserUpdate()
        {
            User u = new User();
            u.Accid = "9a4826e0310e4b78b88daff8e104f516";
            u.Name = "c6396b2d943543aebb0aa5240705be98";


            String json = NimUtil.UserUpdate(u.Accid, null, null, HttpUtility.UrlEncode("另一个昵称1"));
            Console.WriteLine(json);
        }

        private static void testusercreate()
        {
            String accid = Guid.NewGuid().ToString().Replace("-", "");
            Console.WriteLine(accid.Length);
            String json = NimUtil.UserCreate(accid, null, null, HttpUtility.UrlEncode("高级昵称"));
            Answer o = JsonConvert.DeserializeObject<Answer>(json);
            Console.WriteLine(json);
        }

        private static void CreateMethod(string appKey, string nonce, string curTime, string checkSum)
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("AppKey", appKey);
            headers.Add("Nonce", nonce);
            headers.Add("CurTime", curTime);
            headers.Add("CheckSum", checkSum);
            // headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("accid", Guid.NewGuid().ToString().Replace("-", ""));
            String json = HttpUtil.Post("https://api.netease.im/nimserver/user/create.action", headers, parameters);
            Console.WriteLine(json);
        }

        private static void SelectMethod()
        {
            String appKey = "db75c3901c1a2029d0dd668975b580e0";
            String appSecret = "8b928c19e4cc";
            String nonce = Guid.NewGuid().ToString().Replace("-", "");
            String curTime = Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            String checkSum = EncryptionUtil.Sha1Encode(appSecret + nonce + curTime);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("AppKey", appKey);
            headers.Add("Nonce", nonce);
            headers.Add("CurTime", curTime);
            headers.Add("CheckSum", checkSum);

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("accids", "['d5657d6325fe458b9a1f0218a29bc3ca','9a4826e0310e4b78b88daff8e104f516']");
            String json = HttpUtil.Post("https://api.netease.im/nimserver/user/getUinfos.action", headers, parameters);

            Console.WriteLine(json);
        }

        private static String Sha1Encode(String value)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] s = Encoding.UTF8.GetBytes(value);
            Byte[] o = sha1.ComputeHash(s);

            StringBuilder sb = new StringBuilder();

            foreach (var item in o)
            {
                sb.Append(String.Format("{0:x2}", item));
            }
            return sb.ToString();
        }

        private static void Mp3Test()
        {
            FileInfo info = new FileInfo(@"D:\TTPmusic\huanzi.mp3");
            Stream input = info.Open(FileMode.Open);
            Mp3Frame frame = Mp3Frame.LoadFromStream(input);
            Mp3WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2, frame.FrameLength, frame.BitRate);
            Mp3FileReader mp3 = new Mp3FileReader(info.FullName, ff => (new DmoMp3FrameDecompressor(waveFormat)));

            Console.WriteLine(mp3.TotalTime.TotalMilliseconds);//283585

            Mp3FileReader d = new Mp3FileReader(input);
            WaveOut wave = new WaveOut();
            wave.Init(mp3);
            wave.Play();
            Console.WriteLine("{0}:{1}", mp3.TotalTime.Minutes, mp3.TotalTime.Seconds);
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
