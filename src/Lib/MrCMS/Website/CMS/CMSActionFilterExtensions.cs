using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace MrCMS.Website.CMS
{
    public static class CMSActionFilterExtensions
    {
        public static bool IsCMSRequest(this ActionContext context)
        {
            return context.RouteData.DataTokens.ContainsKey(CmsRouter.RouteName);
        }

        public static bool IsCMSRequest(this IHtmlHelper helper)
        {
            return helper.ViewContext.RouteData.DataTokens.ContainsKey(CmsRouter.RouteName);
        }

        public static void MakeCMSRequest(this HttpContext context)
        {
            var routeData = context.GetRouteData();
            routeData?.MakeCMSRequest();
        }

        public static void MakeCMSRequest(this RouteData routeData)
        {
            if (routeData != null)
                routeData.DataTokens[CmsRouter.RouteName] = true;
        }

        public static bool IsPreview(this ActionContext context)
        {
            return context.RouteData.DataTokens.ContainsKey(CmsRouter.IsPreview);
        }

        public static bool IsPreview(this IHtmlHelper helper)
        {
            return helper.ViewContext.RouteData.DataTokens.ContainsKey(CmsRouter.IsPreview);
        }

        public static void MakePreview(this HttpContext context)
        {
            var routeData = context.GetRouteData();
            routeData?.MakePreview();
        }

        public static void MakePreview(this RouteData routeData)
        {
            if (routeData != null)
                routeData.DataTokens[CmsRouter.IsPreview] = true;
        }
    }
}