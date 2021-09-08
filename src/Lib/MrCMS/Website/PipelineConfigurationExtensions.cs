using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Website.CMS;

namespace MrCMS.Website
{
    public static class PipelineConfigurationExtensions
    {
        public static IApplicationBuilder UseMrCMS(this IApplicationBuilder app, Action<IApplicationBuilder> coreApp,
            Action<IEndpointRouteBuilder> coreEndpoint)
        {
            var appContext = app.ApplicationServices.GetRequiredService<MrCMSAppContext>();
            foreach (var part in app.ApplicationServices.GetRequiredService<IGetMrCMSParts>()
                .GetSortedMiddleware(appContext, coreApp))
            {
                part.Registration(app);
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(builder =>
            {
                foreach (var part in app.ApplicationServices.GetRequiredService<IGetMrCMSParts>()
                    .GetSortedEndpoints(appContext, coreEndpoint))
                {
                    part.Registration(builder);
                }
            });

            return app;
        }
    }
}