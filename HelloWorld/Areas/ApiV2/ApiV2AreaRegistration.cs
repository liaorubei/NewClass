using System.Web.Mvc;

namespace StudyOnline.Areas.ApiV2
{
    public class ApiV2AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ApiV2";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ApiV2_default",
                "ApiV2/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}