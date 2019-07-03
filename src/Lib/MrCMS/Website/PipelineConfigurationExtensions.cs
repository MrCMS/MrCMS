using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Website.CMS;

namespace MrCMS.Website
{
    public static class PipelineConfigurationExtensions
    {
        public static IApplicationBuilder UseMrCMS(this IApplicationBuilder app, Action<IApplicationBuilder> coreFunctions)
        {
            var appContext = app.ApplicationServices.GetRequiredService<MrCMSAppContext>();
            foreach (var part in app.ApplicationServices.GetRequiredService<IGetMrCMSParts>()
                .GetSortedMiddleware(appContext, coreFunctions))
            {
                part.Registration(app);
            }

            return app;
        }
    }
}