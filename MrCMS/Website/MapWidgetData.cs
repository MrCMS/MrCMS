using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public class MapWidgetData : IMapWidgetData
    {
        public List<WidgetData> MapData(IList<Widget> widgets)
        {
            return widgets.OrderBy(x => x.DisplayOrder)
                .Select(widget => new WidgetData
                {
                    Id = widget.Id,
                    Name = widget.Name
                })
                .ToList();
        }
    }
}