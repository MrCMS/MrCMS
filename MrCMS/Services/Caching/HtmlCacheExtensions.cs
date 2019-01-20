using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Services.Caching
{
    public static class HtmlCacheExtensions
    {
        public static MvcHtmlString GetCached(this IHtmlHelper helper, CachingInfo cachingInfo,
            Func<IHtmlHelper, MvcHtmlString> func)
        {
            return helper.Get<IHtmlCacheService>().GetString(helper, cachingInfo, func);
        }
        public static MvcHtmlString GetCached(this HtmlHelper helper, CachingInfo cachingInfo,
            Func<HtmlHelper, MvcHtmlString> func)
        {
            var wrappedHtml = helper.GetWrappedHtml();
            return wrappedHtml.Get<IHtmlCacheService>().GetString(wrappedHtml, cachingInfo, htmlHelper => func(helper));
        }
    }
}