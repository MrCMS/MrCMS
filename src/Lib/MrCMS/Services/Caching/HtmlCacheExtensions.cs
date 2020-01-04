using System;
using System.Threading.Tasks;
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
        public static Task<IHtmlContent> GetCached(this IHtmlHelper helper, CachingInfo cachingInfo,
            Func<IHtmlHelper, Task<IHtmlContent>> func)
        {
            return helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IHtmlCacheService>()
                .GetContent(helper, cachingInfo, htmlHelper => func(helper));
        }
    }
}