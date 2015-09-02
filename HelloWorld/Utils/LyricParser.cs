using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StudyOnline.Utils
{
    public class LyricParser
    {
        /// <summary>
        /// 解析歌词
        /// </summary>
        /// <param name="lrc">原始的歌词</param>
        /// <returns>一个已经解析好的歌词对象</returns>
        public static Lyric Parse(String lrc)
        {
            String signPattern = @"\[(?<sign>[a-zA-Z]{2,}):(?<content>.*)\]";//标记标签正则
            String linePattern = @"(\[\d{2,}:\d{2}(\.\d+)*\])+(?<content>.*)";//时间标签+歌词正则
            String timePattern = @"\[(?<minute>\d{2,}):(?<second>\d{2}(\.\d+)*)\]";//标间标签正则

            Lyric lyric = new Lyric();
            lyric.Signs = new Dictionary<string, string>();
            lyric.Lines = new List<KeyValuePair<TimeSpan, string>>();

            //标记标签
            MatchCollection signMC = Regex.Matches(lrc, signPattern);

            foreach (Match item in signMC)
            {
                String sign = item.Groups["sign"].Value;
                String content = item.Groups["content"].Value;
                lyric.Signs.Add(sign, content);
            }

            //时间标签+歌词
            MatchCollection lineMC = Regex.Matches(lrc, linePattern);

            foreach (Match item in lineMC)
            {
                String content = item.Groups["content"].Value;

                //时间标签
                MatchCollection timeMC = Regex.Matches(item.Value, timePattern);
                foreach (Match t in timeMC)
                {
                    double minutes = Convert.ToDouble(t.Groups["minute"].Value);
                    double seconds = Convert.ToDouble(t.Groups["second"].Value);
                    lyric.Lines.Add(new KeyValuePair<TimeSpan, String>(TimeSpan.FromSeconds(minutes * 60 + seconds), content));
                }
            }
            return lyric;
        }
    }

    public class Lyric
    {
        public Dictionary<String, String> Signs { get; set; }
        public List<KeyValuePair<TimeSpan, String>> Lines { get; set; }
    }

    public class Line {
        /// <summary>
        /// 时间标签
        /// </summary>
        public TimeSpan TimeLabel { get; set; }

        /// <summary>
        /// 原文
        /// </summary>
        public String   Original { get; set; }

        /// <summary>
        /// 翻译
        /// </summary>
        public String   Translate { get; set; }
    }
}
