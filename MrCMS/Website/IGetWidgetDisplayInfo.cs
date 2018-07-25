using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Website
{
    public interface IGetWidgetDisplayInfo
    {
        IDictionary<string, WidgetDisplayInfo> GetWidgets(Layout layout);
    }
}