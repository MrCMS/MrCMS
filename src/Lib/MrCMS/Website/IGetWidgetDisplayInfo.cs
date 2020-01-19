using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IGetWidgetDisplayInfo
    {
        Task<IDictionary<string, WidgetDisplayInfo>> GetWidgets(Layout layout, Webpage webpage);
    }
}