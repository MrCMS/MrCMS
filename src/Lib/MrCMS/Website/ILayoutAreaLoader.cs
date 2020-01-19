using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Website
{
    public interface ILayoutAreaLoader
    {
        Task<IEnumerable<LayoutArea>> GetLayoutAreas(int id);
        Task<IEnumerable<LayoutArea>> GetLayoutAreas(Layout layout);
    }
}