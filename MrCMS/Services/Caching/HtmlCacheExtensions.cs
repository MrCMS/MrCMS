using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Services.Caching
{
    public static class HtmlCacheExtensions
    {
        public static IHtmlContent GetCached(this IHtmlHelper helper, CachingInfo cachingInfo,
            Func<IHtmlHelper, IHtmlContent> func)
        {
            return helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IHtmlCacheService>().GetString(helper, cachingInfo, htmlHelper => func(helper));
        }
    }
}