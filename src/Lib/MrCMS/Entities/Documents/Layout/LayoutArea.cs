using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MrCMS.Entities.Documents.Layout
{
    public class LayoutArea : SiteEntity
    {
        public LayoutArea()
        {
            Widgets = new List<Widget.Widget>();
        }
        public virtual Layout Layout { get; set; } //which layout does the area belong to?

        [Required]
        [StringLength(250)]
        public virtual string AreaName { get; set; } // IE. Area.Top, Area.Sidebar.First

        public virtual IList<Widget.Widget> Widgets { get; set; }

        public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }

        public virtual IList<Widget.Widget> GetWidgets(Webpage webpage = null, bool showHidden = false)
        {
            return GetVisibleWidgets(Id, webpage, Widgets.ToList(), showHidden);
        }

        public static List<Widget.Widget> GetVisibleWidgets(int areaId, Webpage webpage, IReadOnlyList<Widget.Widget> allWidgets, bool showHidden = false)
        {
            var widgets = allWidgets.Where(widget => widget.Webpage == null).ToList();

            if (webpage != null)
            {
                var page = webpage;

                widgets.AddRange(allWidgets.Where(widget => widget.Webpage != null && widget.Webpage.Id == page.Id));

                while ((page.Parent.Unproxy() as Webpage) != null)
                {
                    page = page.Parent.Unproxy() as Webpage;

                    widgets.AddRange(
                        allWidgets.Where(widget => widget.Webpage != null && widget.Webpage.Id == page.Id && widget.IsRecursive));
                }

                var widgetsToRemove = new List<Widget.Widget>();

                if (!showHidden)
                {
                    widgetsToRemove.AddRange(widgets.Where(webpage.IsHidden));
                }

                foreach (var widget in widgetsToRemove)
                {
                    widgets.Remove(widget);
                }

                if (webpage.PageWidgetSorts != null)
                {
                    var pageWidgetSorts =
                        webpage.PageWidgetSorts.Where(sort => sort.LayoutArea?.Id == areaId).OrderBy(sort => sort.Order).ToList();

                    if (pageWidgetSorts.Any())
                    {
                        widgets =
                            widgets.OrderByDescending(
                                    widget => pageWidgetSorts.Select(sort => sort.Widget.Id).Contains(widget.Id)).ThenBy(
                                    widget => pageWidgetSorts.Select(sort => sort.Widget.Id).ToList().IndexOf(widget.Id))
                                .ToList();
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
