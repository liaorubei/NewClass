﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Models
{
    public class LoginModel
    {
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string UserName { get; set; }
    }
}