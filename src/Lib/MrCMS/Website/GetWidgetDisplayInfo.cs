using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Website
{
    public class GetWidgetDisplayInfo : IGetWidgetDisplayInfo
    {
        private readonly ILayoutAreaLoader _layoutAreaLoader;
        private readonly IMapWidgetDisplayInfo _mapWidgetDisplayInfo;

        public GetWidgetDisplayInfo(ILayoutAreaLoader layoutAreaLoader,
            IMapWidgetDisplayInfo mapWidgetDisplayInfo
        )
        {
            _layoutAreaLoader = layoutAreaLoader;
            _mapWidgetDisplayInfo = mapWidgetDisplayInfo;
        }

        public async Task<IDictionary<string, WidgetDisplayInfo>> GetWidgets(Layout layout, Webpage webpage)
        {
            var layoutAreas = await _layoutAreaLoader.GetLayoutAreas(layout);

            return await _mapWidgetDisplayInfo.MapInfo(layoutAreas, webpage);
        }
    }
}