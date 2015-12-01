using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChat.Library
{
    public class EncryptionUtil
    {
        public static string Sha1Encode(string value)
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

        public static string Md5Encode(string value)
        {
            MD5 md5 = MD5.Create();
            Byte[] s = Encoding.UTF8.GetBytes(value);
            Byte[] o = md5.ComputeHash(s);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < o.Length; i++)
            {
                sb.Append(o[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public static bool VerifyMd5(String input, String hash)
        {
            String hashOfInput = Md5Encode(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(hashOfInput, hash);
        }
    }
}
