using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Website.Caching;

namespace MrCMS.Website
{
    public class GetWidgetDisplayInfo : IGetWidgetDisplayInfo
    {
        private readonly IMapWidgetDisplayInfo _mapWidgetDisplayInfo;
        private readonly ICacheManager _cacheManager;
        public const string Prefix = "widget-display-info.";

        public GetWidgetDisplayInfo(IMapWidgetDisplayInfo mapWidgetDisplayInfo,
            ICacheManager cacheManager)
        {
            _mapWidgetDisplayInfo = mapWidgetDisplayInfo;
            _cacheManager = cacheManager;
        }

        public async Task<IDictionary<string, WidgetDisplayInfo>> GetWidgets(Layout layout)
        {
            return await _cacheManager.GetOrCreateAsync($"{Prefix}.{layout.Id}", async () =>
            {
                var layoutAreas = layout.GetLayoutAreas();

                return await _mapWidgetDisplayInfo.MapInfo(layoutAreas);
            }, TimeSpan.FromMinutes(5), CacheExpiryType.Sliding);
        }
    }
}