using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Models
{
    public class LoginModel
    {
        public string Password { get; internal set; }
        public bool RememberMe { get; internal set; }
        public string UserName { get; internal set; }
    }
}