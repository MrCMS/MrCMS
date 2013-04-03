using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Core.Widgets
{
    [MrCMSMapClass]
    public class PlainTextWidget : Widget
    {
        public virtual string Text { get; set; }
    }
}