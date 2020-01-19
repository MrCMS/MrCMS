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
        public int LayoutId { get; set; }
        public virtual Layout Layout { get; set; } //which layout does the area belong to?

        [Required]
        [StringLength(250)]
        public virtual string AreaName { get; set; } // IE. Area.Top, Area.Sidebar.First

        public virtual IList<Widget.Widget> Widgets { get; set; }

        //public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }

        //public virtual IList<Widget.Widget> GetWidgets(Webpage webpage = null, bool showHidden = false)
        //{
        //    return 
        //}


        public virtual void AddWidget(Widget.Widget widget)
        {
            Widgets.Add(widget);
        }
    }
}
