using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public interface IGetWidgetsForAreas
    {
        IDictionary<LayoutArea, IList<Widget>> GetWidgets(IEnumerable<LayoutArea> layoutAreas, Webpage webpage);
    }
}