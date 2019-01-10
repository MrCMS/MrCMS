using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Core.Widgets
{
    //[WidgetOutputCacheable]
    // TODO: widget caching
    public class TextWidget : Widget
    {
        public virtual string Text { get; set; }
    }
}