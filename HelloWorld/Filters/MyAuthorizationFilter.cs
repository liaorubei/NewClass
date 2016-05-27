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
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult() { Data = new { statusCode = "301", message = "会话超时", navTabId = "", callbackType = "", forwardUrl = "" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Home/Login");
                }
            }
        }
    }
}