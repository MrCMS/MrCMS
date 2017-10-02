using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class WebpageRoutingExtensions
    {
        public static string RouteWebpage(this UrlHelper url, Webpage webpage, object routeValues = null,
            string protocol = null)
        {
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            values["data"] = webpage?.LiveUrlSegment;
            return url.RouteUrl(MrCMSRouteRegistration.Default, values, protocol, null);
        }

        public static string RouteUniquePage<T>(this UrlHelper url, object routeValues = null,
            string protocol = null) where T : Webpage, IUniquePage
        {
            var service = url.Get<IUniquePageService>();
            var webpage = service.GetUniquePage<T>();
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            values["data"] = webpage?.LiveUrlSegment;
            return url.RouteUrl(MrCMSRouteRegistration.Default, values, protocol, null);
        }
    }
}