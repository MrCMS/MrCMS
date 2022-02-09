using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MrCMS.Apps;

namespace MrCMS.Website.CMS
{
    public static class CmsRouterExtensions
    {
        public static IEndpointRouteBuilder MapMrCMS(this IEndpointRouteBuilder routeBuilder,
            MrCMSAppContext appContext)
        {
            routeBuilder.MapDynamicControllerRoute<MrCMSRouteTransformer>("{**data}", null, 1000);
            foreach (var app in appContext.Apps)
                app.MapRoutes(routeBuilder);

            return routeBuilder;
        }
    }
}