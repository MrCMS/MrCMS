using MrCMS.Web.Apps.Admin.Models.Tabs;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models
{
    public class PlainTextWidgetModel : IUpdatePropertiesViewModel<PlainTextWidget>, IAddPropertiesViewModel<PlainTextWidget>
    {
        public string Text { get; set; }
    }
}