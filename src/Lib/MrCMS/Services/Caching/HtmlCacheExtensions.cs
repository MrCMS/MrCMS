using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Models;

namespace MrCMS.Services.Caching
{
    public static class HtmlCacheExtensions
    {
        public static IHtmlContent GetCached(this IHtmlHelper helper, CachingInfo cachingInfo,
            Func<IHtmlHelper, IHtmlContent> func)
        {
            return helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IHtmlCacheService>().GetContent(helper, cachingInfo, htmlHelper => func(helper));
        }
    }
}