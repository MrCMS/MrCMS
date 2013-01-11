using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Entities.Documents.Web
{
    public class PageWidgetSort : BaseEntity
    {
        public virtual Webpage Webpage { get; set; }
        public virtual LayoutArea LayoutArea { get; set; }
        public virtual Widget.Widget Widget { get; set; }
        public virtual int Order { get; set; }

        public override void OnDeleting()
        {
            if (LayoutArea.PageWidgetSorts.Contains(this))
                LayoutArea.PageWidgetSorts.Remove(this);
            if (Webpage.PageWidgetSorts.Contains(this))
                Webpage.PageWidgetSorts.Remove(this);
            if (Widget.PageWidgetSorts.Contains(this))
                Widget.PageWidgetSorts.Remove(this);
        }
    }
}