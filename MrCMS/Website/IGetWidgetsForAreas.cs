using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public interface IGetWidgetsForAreas
    {
        IDictionary<LayoutArea, IList<Widget>> GetWidgets(IEnumerable<LayoutArea> layoutAreas);
    }
}