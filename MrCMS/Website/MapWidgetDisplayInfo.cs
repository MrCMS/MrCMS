using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public class MapWidgetDisplayInfo : IMapWidgetDisplayInfo
    {
        private readonly IMapWidgetData _mapWidgetData;

        public MapWidgetDisplayInfo(IMapWidgetData mapWidgetData)
        {
            _mapWidgetData = mapWidgetData;
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
    }
}