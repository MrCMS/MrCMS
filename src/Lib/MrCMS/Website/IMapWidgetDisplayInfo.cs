using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public interface IMapWidgetDisplayInfo
    {
        IDictionary<string, WidgetDisplayInfo> MapInfo(IDictionary<LayoutArea, IList<Widget>> widgetData);
        Task<IDictionary<string, WidgetDisplayInfo>> MapInfo(IEnumerable<LayoutArea> layoutAreas, Webpage webpage);
    }
}