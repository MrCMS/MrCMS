using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Models
{
    public class PageWidgetSortModel
    {
        public PageWidgetSortModel()
        {
        }

        public PageWidgetSortModel(IList<Widget> widgets, LayoutArea area, Webpage webpage = null)
        {
            Widgets =
                widgets.Select(
                    (widget, i) =>
                        new WidgetModel
                        {
                            Id = widget.Id,
                            Order = i,
                            Name =
                                !string.IsNullOrWhiteSpace(widget.Name)
                                    ? string.Format("{0} ({1})", widget.Name, widget.WidgetTypeFormatted)
                                    : widget.WidgetTypeFormatted
                        }).ToList();
            if (webpage != null) WebpageId = webpage.Id;
            LayoutAreaId = area.Id;
        }

        public List<WidgetModel> Widgets { get; set; }

        public int WebpageId { get; set; }
        public int LayoutAreaId { get; set; }

        public class WidgetModel
        {
            public int Id { get; set; }
            public int Order { get; set; }
            public string Name { get; set; }
        }
    }
}