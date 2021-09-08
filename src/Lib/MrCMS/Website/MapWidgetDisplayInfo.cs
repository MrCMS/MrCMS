using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Website
{
    public class MapWidgetDisplayInfo : IMapWidgetDisplayInfo
    {
        private readonly IMapWidgetData _mapWidgetData;
        private readonly IWidgetLoader _widgetLoader;

        public MapWidgetDisplayInfo(IMapWidgetData mapWidgetData,
            IWidgetLoader widgetLoader)
        {
            _mapWidgetData = mapWidgetData;
            _widgetLoader = widgetLoader;
        }

        public async Task<IDictionary<string, WidgetDisplayInfo>> MapInfo(IEnumerable<LayoutArea> layoutAreas)
        {
            var dictionary = new Dictionary<string, WidgetDisplayInfo>();
            foreach (var area in layoutAreas)
            {
                dictionary[area.AreaName] =
                    new WidgetDisplayInfo
                    {
                        Id = area.Id,
                        Name = area.AreaName,
                        Widgets = _mapWidgetData.MapData(await _widgetLoader.GetWidgets(area))
                    };
            }

            return dictionary;
        }
    }
}