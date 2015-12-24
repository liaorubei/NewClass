using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChat.Library
{
    public class Uinfo
    {
        public String Accid { get; set; }   //是	用户帐号，最大长度32字节，必须保证一个APP内唯一
        public String Name { get; set; }    //否	用户昵称，最大长度64字节
        public String Icon { get; set; }    //否	用户icon，最大长度256字节
        public String Sign { get; set; }    //否	用户签名，最大长度256字节
        public String Email { get; set; }   //否	用户email，最大长度64字节
        public String Birth { get; set; }   //否	用户生日，最大长度16字节
        public String Mobile { get; set; }  //否	用户mobile，最大长度32字节
        public String Gender { get; set; }  //否	用户性别，0表示未知，1表示男，2女表示女，其它会报参数错误
        public String Ex { get; set; }      //否	用户名片扩展字段，最大长度1024字节，用户可自行扩展，建议封装成JSON字符串
    }
}
