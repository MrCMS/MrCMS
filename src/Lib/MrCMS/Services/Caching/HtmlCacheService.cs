using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website.Caching;

namespace MrCMS.Services.Caching
{
    public class HtmlCacheService : IHtmlCacheService
    {
        private readonly ICacheManager _cacheManager;

        public HtmlCacheService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }


        public IHtmlContent GetContent(Controller controller, CachingInfo cachingInfo,
            Func<IHtmlHelper, IHtmlContent> func)
        {
            return _cacheManager.GetOrCreate(cachingInfo.CacheKey, () => func(controller.GetHtmlHelper()),
                cachingInfo.ShouldCache ? cachingInfo.TimeToCache : TimeSpan.Zero, cachingInfo.ExpiryType);
        }
        public IHtmlContent GetContent(IHtmlHelper helper, CachingInfo cachingInfo, Func<IHtmlHelper, IHtmlContent> func)
        {
            return _cacheManager.GetOrCreate(cachingInfo.CacheKey, () => func(helper),
                cachingInfo.ShouldCache ? cachingInfo.TimeToCache : TimeSpan.Zero, cachingInfo.ExpiryType);
        }

        public async Task<IHtmlContent> GetContent(IViewComponentHelper helper, CachingInfo cachingInfo,
            Func<IViewComponentHelper, Task<IHtmlContent>> func)
        {
            return await _cacheManager.GetOrCreateAsync(cachingInfo.CacheKey, async () =>
                {
                    var htmlContent = await func(helper);
                    var stringBuilder = new StringBuilder();
                    TextWriter writer = new StringWriter(stringBuilder);
                    htmlContent.WriteTo(writer, HtmlEncoder.Default);
                    IHtmlContent content = new HtmlString(stringBuilder.ToString());
                    return content;
                },
                cachingInfo.ShouldCache ? cachingInfo.TimeToCache : TimeSpan.Zero, cachingInfo.ExpiryType);
        }
    }
}