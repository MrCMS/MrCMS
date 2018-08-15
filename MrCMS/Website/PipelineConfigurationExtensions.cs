using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Website.CMS;
using System;

namespace MrCMS.Website
{
    public static class PipelineConfigurationExtensions
    {
        public static IApplicationBuilder UseMrCMS(this IApplicationBuilder app)
        {
            var appContext = app.ApplicationServices.GetRequiredService<MrCMSAppContext>();
            foreach (var middleware in app.ApplicationServices.GetRequiredService<IGetMrCMSMiddleware>().GetSortedMiddleware(appContext))
            {
                middleware.Registration(app);
            }

            app.UseSignalR(routes =>
            {
                var methodInfo = typeof(HubRouteBuilder).GetMethod(nameof(HubRouteBuilder.MapHub), new Type[] { typeof(PathString) });
                var signalRRoutes = appContext.SignalRHubs;
                foreach (var key in signalRRoutes.Keys)
                {
                    var pathString = new PathString(signalRRoutes[key]);
                    methodInfo.MakeGenericMethod(key).Invoke(routes, new object[] { pathString });
                }
            });

            app.UseMvc(builder =>
            {
                builder.MapMrCMS();
                builder.MapMrCMSApps(appContext);

                builder.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

            return app;
        }
    }
}