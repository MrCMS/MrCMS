using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public class MapWidgetDisplayInfo : IMapWidgetDisplayInfo
    {
        private readonly IMapWidgetData _mapWidgetData;
        private readonly IRepository<PageWidgetSort> _pageWidgetSortRepository;
        private readonly IWidgetLoader _widgetLoader;

        public MapWidgetDisplayInfo(IMapWidgetData mapWidgetData, IRepository<PageWidgetSort> pageWidgetSortRepository, IWidgetLoader widgetLoader)
        {
            _mapWidgetData = mapWidgetData;
            _pageWidgetSortRepository = pageWidgetSortRepository;
            _widgetLoader = widgetLoader;
        }

        public IDictionary<string, WidgetDisplayInfo> MapInfo(IDictionary<LayoutArea, IList<Widget>> widgetData)
        {
            return widgetData.ToDictionary(area => area.Key.AreaName,
                pair =>
                {
                    var area = pair.Key;

                    return new WidgetDisplayInfo
                    {
                        Id = area.Id,
                        Name = area.AreaName,
                        Widgets = _mapWidgetData.MapData(pair.Value)
                    };
                }, StringComparer.OrdinalIgnoreCase);

        }

        public async Task<IDictionary<string, WidgetDisplayInfo>> MapInfo(IEnumerable<LayoutArea> layoutAreas,
            Webpage webpage)
        {
            var pageWidgetSorts = await _pageWidgetSortRepository.Readonly().Where(x => x.WebpageId == webpage.Id).ToListAsync();
            var dictionary = new Dictionary<string, WidgetDisplayInfo>();

            foreach (var area in layoutAreas)
            {
                dictionary[area.AreaName] = 
                     new WidgetDisplayInfo
                    {
                        Id = area.Id,
                        Name = area.AreaName,
                        HasCustomSort = pageWidgetSorts?.Any(x => x.LayoutAreaId == area.Id) == true,
                        Widgets = _mapWidgetData.MapData(await _widgetLoader.GetWidgets(area,webpage))
                    };
            }

            return dictionary;
        }
    }
}