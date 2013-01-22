using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Entities.Documents.Layout
{
    public class LayoutArea : SiteEntity
    {
        public virtual Layout Layout { get; set; } //which layout does the area belong to?
        public virtual string AreaName { get; set; } // IE. Area.Top, Area.Sidebar.First

        protected internal virtual IList<Widget.Widget> Widgets { get; set; }

        public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }

        public virtual List<Widget.Widget> GetWidgets(Webpage webpage = null, bool showHidden = false)
        {
            var widgets = Widgets.Where(widget => widget.Webpage == null).ToList();

            if (webpage != null)
            {
                var page = webpage;

                widgets.AddRange(Widgets.Where(widget => widget.Webpage != null && widget.Webpage.Id == page.Id));

                while ((page.Parent.Unproxy() as Webpage) != null)
                {
                    page = page.Parent.Unproxy() as Webpage;

                    widgets.AddRange(
                        Widgets.Where(widget => widget.Webpage != null && widget.Webpage.Id == page.Id && widget.IsRecursive));
                }

                var widgetsToRemove = new List<Widget.Widget>();

                if (!showHidden)
                    widgetsToRemove.AddRange(widgets.Where(webpage.IsHidden));

                foreach (var widget in widgetsToRemove)
                    widgets.Remove(widget);

                if (webpage.PageWidgetSorts != null)
                {
                    var pageWidgetSorts =
                        webpage.PageWidgetSorts.Where(sort => sort.LayoutArea == this).OrderBy(sort => sort.Order).ToList();

                    if (pageWidgetSorts.Any())
                    {
                        var list =
                            widgets.OrderByDescending(
                                widget => pageWidgetSorts.Select(sort => sort.Widget.Id).Contains(widget.Id)).ThenBy(
                                    widget => pageWidgetSorts.Select(sort => sort.Widget.Id).ToList().IndexOf(widget.Id))
                                   .ToList();
                        return list;
                    }
                }
            }
            else
            {
                widgets.RemoveAll(widget => widget == null);
                widgets.Sort((widget1, widget2) => widget1.DisplayOrder - widget2.DisplayOrder);
            }

            return widgets;
        }

        public virtual void AddWidget(Widget.Widget widget)
        {
            Widgets.Add(widget);
        }
    }
}
