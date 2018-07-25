using System.ComponentModel;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Widgets
{
    //[WidgetOutputCacheable(PerPage = true)]
    // TODO: widget caching
    public class CurrentPageSubNavigation : Widget
    {
        [DisplayName("Show Name As Title")]
        public virtual bool ShowNameAsTitle { get; set; }
    }
}