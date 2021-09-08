using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Website
{
    public interface IMapWidgetDisplayInfo
    {
        Task<IDictionary<string, WidgetDisplayInfo>> MapInfo(IEnumerable<LayoutArea> layoutAreas);
    }
}