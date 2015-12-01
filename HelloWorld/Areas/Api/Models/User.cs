using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Areas.Api.Models
{
    public class User
    {
        /// <summary>
        /// 登录ID
        /// </summary>
        public String accid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// json属性
        /// </summary>
        public String props { get; set; }
        /// <summary>
        /// icon地址
        /// </summary>
        public String icon { get; set; }
        /// <summary>
        /// 登录凭证
        /// </summary>
        public String token { get; set; }
    }
}