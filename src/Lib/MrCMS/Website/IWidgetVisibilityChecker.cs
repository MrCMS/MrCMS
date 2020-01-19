using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public interface IWidgetVisibilityChecker
    {
        Task<bool> IsHidden(Webpage webpage, Widget widget);
        Task<int> CountVisible(IEnumerable<Widget> widgets, Webpage webpage);
        Task<int> CountHidden(IEnumerable<Widget> widgets, Webpage webpage);
    }
}