using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Website.CMS;

namespace MrCMS.Website
{
    public static class PipelineConfigurationExtensions
    {
        public static IApplicationBuilder UseMrCMS(this IApplicationBuilder app)
        {
            var appContext = app.ApplicationServices.GetRequiredService<MrCMSAppContext>();
            foreach (var part in app.ApplicationServices.GetRequiredService<IGetMrCMSParts>()
                .GetSortedMiddleware(appContext)) part.Registration(app);

            //app.UseSignalR(routes =>
            //{
            //    var methodInfo = typeof(HubRouteBuilder).GetMethod(nameof(HubRouteBuilder.MapHub), new Type[] { typeof(PathString) });
            //    var signalRRoutes = appContext.SignalRHubs;
            //    foreach (var key in signalRRoutes.Keys)
            //    {
            //        var pathString = new PathString(signalRRoutes[key]);
            //        methodInfo.MakeGenericMethod(key).Invoke(routes, new object[] { pathString });
            //    }
            //});

            //app.UseMvc(builder =>
            //{
            //    builder.MapMrCMS();
            //    builder.MapMrCMSApps(appContext);

            //    builder.MapRoute(
            //        "default",
            //        "{controller=Home}/{action=Index}/{id?}");
            //});

            return app;
        }
    }
}