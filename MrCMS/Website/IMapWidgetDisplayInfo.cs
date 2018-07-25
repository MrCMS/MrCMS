using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public interface IMapWidgetDisplayInfo
    {
        IDictionary<string, WidgetDisplayInfo> MapInfo(IDictionary<LayoutArea, IList<Widget>> widgetData);
    }
}