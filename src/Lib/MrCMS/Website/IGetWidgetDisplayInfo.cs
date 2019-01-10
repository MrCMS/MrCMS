using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IGetWidgetDisplayInfo
    {
        IDictionary<string, WidgetDisplayInfo> GetWidgets(Layout layout, Webpage webpage);
    }
}