using Microsoft.AspNetCore.Routing;

namespace MrCMS.Apps
{
    public static class MrCMSAppRouteRegistrationExtensions
    {
        public static IRouteBuilder MapMrCMSApps(this IRouteBuilder routeBuilder, MrCMSAppContext context)
        {
            foreach (var app in context.Apps)
                routeBuilder = app.MapRoutes(routeBuilder);

            return routeBuilder;
        }
    }
}