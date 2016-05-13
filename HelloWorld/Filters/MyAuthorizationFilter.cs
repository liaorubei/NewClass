using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Filters
{
    public class MyAuthorizationFilter : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var CurrentUser = filterContext.HttpContext.Session["CurrentUser"] as User;
            if (CurrentUser == null)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
            }
        }
    }
}