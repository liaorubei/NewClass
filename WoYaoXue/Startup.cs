using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WoYaoXue.Startup))]
namespace WoYaoXue
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
