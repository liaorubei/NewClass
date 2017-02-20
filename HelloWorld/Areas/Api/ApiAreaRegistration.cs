using System.Web.Mvc;
using System.Web.Routing;

namespace StudyOnline.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("Api_default", "Api/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional });
            //context.MapRoute("Api_default", "Api/{controller}/{action}/{id}", new { action = "Index", id = UrlParameter.Optional }, new { HttpMethod = new HttpMethodConstraint("POST") });
        }
    }
}