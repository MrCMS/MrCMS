using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
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
            serviceCollection.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            serviceCollection.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            serviceCollection.RegisterSwagger();

            return base.RegisterServices(serviceCollection);
        }
    }
}
