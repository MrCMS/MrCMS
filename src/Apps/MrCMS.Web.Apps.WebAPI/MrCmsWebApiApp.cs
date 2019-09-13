using System;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.WebApi.Helpers;

namespace MrCMS.Web.Apps.WebApi
{
    public class MrCmsWebApiApp : StandardMrCMSApp
    {
        public MrCmsWebApiApp()
        {
            ContentPrefix = "/Apps/WebApi";
            ViewPrefix = "/Apps/WebApi";
        }
        public override string Name => "WebApi";
        public override string Version => "1.0";

        public override IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterSwagger();
            return base.RegisterServices(serviceCollection);
        }
    }
}
