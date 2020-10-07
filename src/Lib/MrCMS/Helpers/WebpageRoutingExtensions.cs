using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.CMS;

namespace MrCMS.Helpers
{
    public static class WebpageRoutingExtensions
    {
        public static string RouteWebpage(this IUrlHelper url, Webpage webpage, object routeValues = null,
            string protocol = null)
        {
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            values["data"] = webpage?.UrlSegment;
            return url.RouteUrl(CmsRouter.RouteName, values, protocol, null);
        }

        public static string RouteUniquePage<T>(this IUrlHelper url, object routeValues = null,
            string protocol = null) where T : Webpage, IUniquePage
        {
            var service = url.ActionContext.HttpContext.RequestServices.GetRequiredService<IUniquePageService>();
            var webpage = service.GetUniquePage<T>();
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            values["data"] = webpage?.UrlSegment;
            return url.RouteUrl(CmsRouter.RouteName, values, protocol, null);
        }

        public static string RouteWebpage(this IHtmlHelper htmlHelper, Webpage webpage, object routeValues = null,
            string protocol = null)
        {
            var urlHelper = htmlHelper.GetRequiredService<IUrlHelper>();
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            values["data"] = webpage?.UrlSegment;
            return urlHelper.RouteUrl(CmsRouter.RouteName, values, protocol, null);
        }

        public static string RouteUniquePage<T>(this IHtmlHelper htmlHelper, object routeValues = null,
            string protocol = null) where T : Webpage, IUniquePage
        {
            var service = htmlHelper.GetRequiredService<IUniquePageService>();
            var urlHelper = htmlHelper.GetRequiredService<IUrlHelper>();
            var webpage = service.GetUniquePage<T>();
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            values["data"] = webpage?.UrlSegment;
            return urlHelper.RouteUrl(CmsRouter.RouteName, values, protocol, null);
        }
    }
}