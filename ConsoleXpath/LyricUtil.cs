using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleXpath.a
{
    public class LyricUtil
    {
        public static LrcFile Parse(string lrcString)
        {
            LrcFile file = new LrcFile();
            file.Ti = getInfo(lrcString, "");
            file.Al = getInfo(lrcString, "");
            file.Ar = getInfo(lrcString, "");
            file.By = getInfo(lrcString, "");
            file.Lrcs = getLrcs();
            return file;
        }

        private static string getInfo(string lrcString, string p)
        {
            Regex reg = new Regex(@"\[ti:[^\]]*\]", RegexOptions.IgnoreCase);

            Match match = reg.Match(lrcString);
            String d = match.Value;
            return d;
        }



        private static List<Lrc> getLrcs()
        {
            List<Lrc> lrcs = new List<Lrc>();

            return lrcs;
        }
    }

    /// <summary>
    /// 歌词文件,包括标题,专辑,歌手,歌词文件编辑,歌词与时间标签
    /// </summary>
    public class LrcFile
    {
        /// <summary>
        /// 标题,歌名
        /// </summary>
        public String Ti { get; set; }

        /// <summary>
        /// 歌手
        /// </summary>
        public String Ar { get; set; }

        /// <summary>
        /// 专辑
        /// </summary>
        public String Al { get; set; }

        /// <summary>
        /// 歌词文件作者
        /// </summary>
        public String By { get; set; }

        public List<Lrc> Lrcs { get; set; }
    }

    /// <summary>
    /// 歌词句子,包括时间标签,外文歌词,中文歌词
    /// </summary>
    public class Lrc
    {
        /// <summary>
        /// 时间标签
        /// </summary>
        public TimeSpan Ts { get; set; }

        /// <summary>
        /// 外文歌词
        /// </summary>
        public String En { get; set; }

        /// <summary>
        /// 中文歌词
        /// </summary>
        public String Cn { get; set; }
    }
}
