using MrCMS.Apps;
using NHibernate.Cfg;
using System;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Web.Apps.IdentityServer4.Admin.ExceptionHandling;

namespace MrCMS.Web.Apps.IdentityServer4.Admin
{
    public class MrCMSIs4AdminApp : StandardMrCMSApp
    {
        public MrCMSIs4AdminApp()
        {
            ContentPrefix = "/Apps/IS4Admin";
            ViewPrefix = "/Apps/IS4Admin";
        }
        public override string Name => "IS4Admin";
        public override string Version => "1.0";

        

        public override void AppendConfiguration(Configuration configuration)
        {
            //  configuration.
        }

        public override IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        {
            //Exception handling
            serviceCollection.AddScoped<ControllerExceptionFilterAttribute>();
            return serviceCollection;
        }

    }
}
