using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
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
            var hubActivator = new MrCMSHubActivator();

            GlobalHost.DependencyResolver.Register(
                typeof(IHubActivator),
                () => hubActivator);

            app.ConfigureAuth();
            
            ConfigureSignalR(app);
        }
        public static void ConfigureSignalR(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }

    public class MrCMSHubActivator : IHubActivator
    {
        public IHub Create(HubDescriptor descriptor)
        {
            return MrCMSApplication.Get(descriptor.HubType) as IHub;
        }
    }

}
