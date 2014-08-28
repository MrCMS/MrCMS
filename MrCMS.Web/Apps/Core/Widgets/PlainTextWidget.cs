using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Widgets
{
    [OutputCacheable]
    public class PlainTextWidget : Widget
    {
        public virtual string Text { get; set; }
    }
}