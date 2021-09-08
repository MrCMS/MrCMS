using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.Website
{
    public class MapWidgetData : IMapWidgetData
    {
        public List<WidgetData> MapData(IList<Widget> widgets)
        {
            return widgets
                .Select(widget => new WidgetData
                {
                    Id = widget.Id,
                    Name = widget.Name,
                    ViewName = widget.Unproxy().GetType().Name
                })
                .ToList();
        }
    }
}