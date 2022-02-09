using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Layout
{
    public class LayoutArea : SiteEntity
    {
        public LayoutArea()
        {
            Widgets = new List<Widget.Widget>();
        }

        public virtual Layout Layout { get; set; } //which layout does the area belong to?

        [Required] [StringLength(250)] public virtual string AreaName { get; set; } // IE. Area.Top, Area.Sidebar.First

        public virtual IList<Widget.Widget> Widgets { get; set; }



        public virtual void AddWidget(Widget.Widget widget)
        {
            Widgets.Add(widget);
        }
    }
}