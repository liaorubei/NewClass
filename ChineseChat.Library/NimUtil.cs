using Newtonsoft.Json;
using System;
using System.Collections.Specialized;

namespace ChineseChat.Library
{
    public class NimUtil
    {
        public static readonly string AppKey = "db75c3901c1a2029d0dd668975b580e0";
        public static readonly string AppSecret = "8b928c19e4cc";
        public readonly static string UserCreatePath = "https://api.netease.im/nimserver/user/create.action";

        public static Answer UserCreate(User user)
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

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("accid", user.Accid);
            parameters.Add("name", user.Name);
            parameters.Add("icon", user.Icon);
            parameters.Add("token", user.Token);//云信Token为固定算出来的值,不与帐号的密码有关联,免得业务变复杂

            String json = HttpUtil.Post(NimUtil.UserCreatePath, headers, parameters);
            Console.WriteLine(json);
            return JsonConvert.DeserializeObject<Answer>(json);
        }



    }
}
