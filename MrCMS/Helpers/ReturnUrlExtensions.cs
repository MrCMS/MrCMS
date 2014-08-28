using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Website.Filters;

namespace MrCMS.Helpers
{
    public static class ReturnUrlExtensions
    {
        public static MvcHtmlString ReturnToReferrer(this HtmlHelper helper)
        {
            if (helper != null && (helper.ViewContext != null &&
                                   (helper.ViewContext.HttpContext != null &&
                                    (helper.ViewContext.HttpContext.Request != null &&
                                     helper.ViewContext.HttpContext.Request.UrlReferrer !=
                                     null))))
                return helper.Hidden(ReturnUrlHandlerAttribute.ReturnUrlKey,
                    helper.ViewContext.HttpContext.Request.UrlReferrer.ToString());
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString ReturnToThis(this HtmlHelper helper)
        {
            if (helper != null && (helper.ViewContext != null &&
                                   (helper.ViewContext.HttpContext != null &&
                                    (helper.ViewContext.HttpContext.Request != null &&
                                     helper.ViewContext.HttpContext.Request.Url !=
                                     null))))
                return helper.Hidden(ReturnUrlHandlerAttribute.ReturnUrlKey,
                    helper.ViewContext.HttpContext.Request.Url.ToString());
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString ReturnTo(this HtmlHelper helper, string url)
        {
            return !string.IsNullOrWhiteSpace(url)
                ? helper.Hidden(ReturnUrlHandlerAttribute.ReturnUrlKey, url)
                : MvcHtmlString.Empty;
        }
    }
}