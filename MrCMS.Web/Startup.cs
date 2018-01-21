using Microsoft.Owin;
using MrCMS.Helpers;
using MrCMS.Web;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace MrCMS.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.ConfigureAuth();
        }
    }
}