using MrCMS.Apps;
using NHibernate.Cfg;
using System;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

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
           // serviceCollection.AddScoped<ControllerExceptionFilterAttribute>();
            //serviceCollection.AddLocalization(options => options.ResourcesPath = "Resources");

            //serviceCollection.AddMvc()
            //    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            //    .AddDataAnnotationsLocalization();
            //  return serviceCollection;
            return base.RegisterServices(serviceCollection);
        }

    }
}
