using Newtonsoft.Json;
using System;
using System.Collections.Specialized;

namespace ChineseChat.Library
{
    public class NimUtil
    {
        public static readonly string AppKey = "db75c3901c1a2029d0dd668975b580e0";
        public static readonly string AppSecret = "8b928c19e4cc";
        private readonly static string UserCreatePath = "https://api.netease.im/nimserver/user/create.action";
        private static readonly string UserUpdatePath = "https://api.netease.im/nimserver/user/update.action";

        public static String UserCreate(String accid, String token, String props, String name)
        {
            NameValueCollection headers = GenerateHeaders();
            NameValueCollection parameters = GenerateParameters(accid, token, props, name);
            return HttpUtil.Post(NimUtil.UserCreatePath, headers, parameters);
        }

        public static String UserUpdate(String accid, String token, String props, String name)
        {
            NameValueCollection headers = GenerateHeaders(); ;
            NameValueCollection parameters = GenerateParameters(accid, token, props, name);
            return HttpUtil.Post(NimUtil.UserUpdatePath, headers, parameters);
        }

        private static NameValueCollection GenerateHeaders()
        {
            String appKey = NimUtil.AppKey;
            String appSecret = NimUtil.AppSecret;
            String nonce = Guid.NewGuid().ToString().Replace("-", "");
            String curTime = Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            String checkSum = EncryptionUtil.Sha1Encode(appSecret + nonce + curTime);

            NameValueCollection headers = new NameValueCollection();
            headers.Add("AppKey", appKey);
            headers.Add("Nonce", nonce);
            headers.Add("CurTime", curTime);
            headers.Add("CheckSum", checkSum);
            return headers;
        }

        private static NameValueCollection GenerateParameters(string accid, string token, string props, string name)
        {
            NameValueCollection n = new NameValueCollection();
            if (!String.IsNullOrEmpty(accid))
            {
                n.Add("accid", accid);
            }
            if (!String.IsNullOrEmpty(token))
            {
                n.Add("token", token);
            }
            if (!String.IsNullOrEmpty(props))
            {
                n.Add("props", props);
            }
            if (!String.IsNullOrEmpty(name))
            {
                n.Add("name", name);
            }
            return n;
        }

    }
}
