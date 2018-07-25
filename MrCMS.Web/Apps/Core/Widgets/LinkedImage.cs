using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Widgets
{
    //[WidgetOutputCacheable]
    // TODO: widget caching
    public class LinkedImage : Widget 
    {
        public virtual string Image { get; set; }
        public virtual string Link { get; set; }
    }
}