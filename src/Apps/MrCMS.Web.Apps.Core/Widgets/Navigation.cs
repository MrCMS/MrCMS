using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Widgets
{
    [WidgetOutputCacheable(PerPage = true)]
    public class Navigation : Widget
    {
        public virtual bool IncludeChildren { get; set; }
    }
}