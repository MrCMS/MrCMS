using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Website.Filters;

namespace MrCMS.Helpers
{
    public static class ReturnUrlExtensions
    {
        public static IHtmlContent ReturnToReferrer(this IHtmlHelper helper)
        {
            if (helper?.ViewContext?.HttpContext?.Request?.Referer() != null)
                return helper.Hidden(ReturnUrlHandlerAttribute.ReturnUrlKey,
                    helper.ViewContext.HttpContext.Request.Referer());
            return HtmlString.Empty;
        }

        public static IHtmlContent ReturnToThis(this IHtmlHelper helper)
        {
            if (helper?.ViewContext?.HttpContext?.Request?.GetEncodedUrl() != null)
                return helper.Hidden(ReturnUrlHandlerAttribute.ReturnUrlKey,
                    helper.ViewContext.HttpContext.Request.GetEncodedUrl());
            return HtmlString.Empty;
        }

        public static IHtmlContent ReturnTo(this IHtmlHelper helper, string url)
        {
            return !string.IsNullOrWhiteSpace(url)
                ? helper.Hidden(ReturnUrlHandlerAttribute.ReturnUrlKey, url)
                : HtmlString.Empty;
        }
    }
}