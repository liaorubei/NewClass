using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebHanWen.Startup))]
namespace WebHanWen
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
