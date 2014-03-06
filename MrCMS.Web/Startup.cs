using Microsoft.Owin;
using MrCMS.Web;
using MrCMS.Website;
using Owin;
using MrCMS.Helpers;

[assembly: OwinStartup(typeof(Startup))]
namespace MrCMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.ConfigureAuth();
        }
    }
}
