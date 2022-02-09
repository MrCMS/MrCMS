using System.Collections.Generic;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public interface IMapWidgetData
    {
        List<WidgetData> MapData(IList<Widget> widgets);
    }
}