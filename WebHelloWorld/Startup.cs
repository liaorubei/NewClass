using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebHelloWorld.Startup))]
namespace WebHelloWorld
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
