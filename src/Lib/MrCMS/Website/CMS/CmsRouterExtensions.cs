using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace MrCMS.Website.CMS
{
    public static class CmsRouterExtensions
    {
        public static IRouteBuilder MapMrCMS(this IRouteBuilder routeBuilder)
        {
            routeBuilder.Routes.Add(new CmsRouter(
                routeBuilder.DefaultHandler
            ));

            routeBuilder.Routes.Add(new UrlHistoryRouter(
                routeBuilder.DefaultHandler
            ));

            return routeBuilder;
        }
    }
}