using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Website
{
    public class GetWidgetDisplayInfo : IGetWidgetDisplayInfo
    {
        private readonly IMapWidgetDisplayInfo _mapWidgetDisplayInfo;

        public GetWidgetDisplayInfo(
            IMapWidgetDisplayInfo mapWidgetDisplayInfo
        )
        {
            _mapWidgetDisplayInfo = mapWidgetDisplayInfo;
        }

        public IDictionary<string, WidgetDisplayInfo> GetWidgets(Layout layout, Webpage webpage)
        {
            var layoutAreas = layout.GetLayoutAreas();

            return _mapWidgetDisplayInfo.MapInfo(layoutAreas, webpage);
        }
    }
}