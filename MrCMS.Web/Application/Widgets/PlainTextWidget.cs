using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Application.Widgets
{
    [MrCMSMapClass]
    public class PlainTextWidget : Widget
    {
        public virtual string Text { get; set; }
    }
}