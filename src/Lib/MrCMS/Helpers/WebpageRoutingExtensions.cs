using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Helpers
{
    public static class WebpageRoutingExtensions
    {
        public static string RouteWebpage(this IUrlHelper url, Webpage webpage, object routeValues = null)
        {
            return GetUrl(webpage, routeValues);
        }

        public static string GetUrl(this Webpage webpage, object routeValues = null)
        {
            var queryString = GetQueryString(routeValues);

            return $"/{webpage.UrlSegment}{queryString}";
        }

        private static QueryString GetQueryString(object routeValues)
        {
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            var queryString = new QueryString();
            foreach (var key in values.Keys)
            {
                var value = values[key];
                if (value is StringValues stringValues)
                {
                    queryString = stringValues.Aggregate(queryString, (current, s) => current.Add(key, s));
                }
                else
                {
                    var stringValue = value?.ToString();
                    if (!string.IsNullOrWhiteSpace(stringValue))
                    {
                        queryString = queryString.Add(key, stringValue);
                    }
                }
            }

            return queryString;
        }

        public static async Task<string> RouteUniquePage<T>(this IUrlHelper url, object routeValues = null)
            where T : Webpage, IUniquePage
        {
            var service = url.ActionContext.HttpContext.RequestServices.GetRequiredService<IUniquePageService>();
            var webpage = await service.GetUniquePage<T>();
            var queryString = GetQueryString(routeValues);
            return $"/{webpage.UrlSegment}{queryString}";
        }

        public static string RouteWebpage(this IHtmlHelper htmlHelper, Webpage webpage, object routeValues = null,
            string protocol = null)
        {
            var urlHelper = htmlHelper.GetRequiredService<IUrlHelper>();
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            values["data"] = webpage?.UrlSegment;
            return urlHelper.RouteUrl(null, values, protocol, null);
        }

        public static async Task<string> RouteUniquePage<T>(this IHtmlHelper htmlHelper, object routeValues = null,
            string protocol = null) where T : Webpage, IUniquePage
        {
            var service = htmlHelper.GetRequiredService<IUniquePageService>();
            var urlHelper = htmlHelper.GetRequiredService<IUrlHelper>();
            var webpage = await service.GetUniquePage<T>();
            var dictionary = routeValues as RouteValueDictionary;
            var values = dictionary ?? new RouteValueDictionary(routeValues);
            values["data"] = webpage?.UrlSegment;
            return urlHelper.RouteUrl(null, values, protocol, null);
        }
    }
}