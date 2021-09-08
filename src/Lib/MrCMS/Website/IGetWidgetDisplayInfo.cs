using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Website
{
    public interface IGetWidgetDisplayInfo
    {
        Task<IDictionary<string, WidgetDisplayInfo>> GetWidgets(Layout layout);
    }
}