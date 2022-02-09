using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;

namespace MrCMS.Models
{
    [DoNotMap]
    public class WidgetSortModel
    {
        public WidgetSortModel()
        {
        }

        public WidgetSortModel(IList<Widget> widgets, LayoutArea area)
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
                                    ? $"{widget.Name} ({widget.WidgetTypeFormatted})"
                                    : widget.WidgetTypeFormatted
                        }).ToList();
            LayoutAreaId = area.Id;
        }

        public List<WidgetModel> Widgets { get; set; }

        public int LayoutAreaId { get; set; }

        public class WidgetModel
        {
            public int Id { get; set; }
            public int Order { get; set; }
            public string Name { get; set; }
        }
    }
}