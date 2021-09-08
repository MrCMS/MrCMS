using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Website.Filters;

namespace MrCMS.Helpers
{
    public static class ReturnUrlExtensions
    {
        public static IHtmlContent ReturnToReferrer(this IHtmlHelper helper)
        {
            var referer = helper?.ViewContext?.HttpContext?.Request?.Referer();
            if (referer != null)
                return helper.Hidden(ReturnUrlAttribute.ReturnUrlKey, referer);
            return HtmlString.Empty;
        }

        public static IHtmlContent ReturnToThis(this IHtmlHelper helper)
        {
            if (helper?.ViewContext?.HttpContext?.Request?.GetEncodedUrl() != null)
                return helper.Hidden(ReturnUrlAttribute.ReturnUrlKey,
                    helper.ViewContext.HttpContext.Request.GetEncodedUrl());
            return HtmlString.Empty;
        }

        public static IHtmlContent ReturnTo(this IHtmlHelper helper, string url)
        {
            return !string.IsNullOrWhiteSpace(url)
                ? helper.Hidden(ReturnUrlAttribute.ReturnUrlKey, url)
                : HtmlString.Empty;
        }
    }
}