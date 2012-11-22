using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Entities.Documents.Web
{
    public class LayoutAreaOverride : BaseDocumentItemEntity
    {
        public virtual Webpage Webpage { get; set; } //which layout does the area belong to?
        public virtual LayoutArea LayoutArea { get; set; } // IE. Area.Top, Area.Sidebar.First
        public virtual Widget.Widget Widget { get; set; } //what widget do we put in area?
        public virtual bool RecurseForChildren { get; set; } //if ticked, all children items will use this same override in the same area
    }
}