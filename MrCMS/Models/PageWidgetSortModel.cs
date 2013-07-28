using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Models
{
    [DoNotMap]
    public class PageWidgetSortModel
    {
        public List<WidgetModel> Widgets { get; set; }

        public class WidgetModel
        {
            public int Id { get; set; }
            public int Order { get; set; }
            public string Name { get; set; }
        }

        public int WebpageId { get; set; }
        public int LayoutAreaId { get; set; }

        public PageWidgetSortModel()
        {
        }

        public PageWidgetSortModel(List<Widget> widgets, LayoutArea area, Webpage webpage = null)
        {
            Widgets =
                widgets.Select(
                    (widget, i) =>
                    new WidgetModel()
                        {
                            Id = widget.Id,
                            Order = i,
                            Name =
                                (!String.IsNullOrWhiteSpace(widget.Name)
                                     ? String.Format("{0} ({1})", widget.Name, widget.WidgetTypeFormatted)
                                     : widget.WidgetTypeFormatted)
                        }).ToList();
            if (webpage != null) WebpageId = webpage.Id;
            LayoutAreaId = area.Id;
        }
    }
}