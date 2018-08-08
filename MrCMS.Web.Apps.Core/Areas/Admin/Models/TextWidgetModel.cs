using MrCMS.Web.Apps.Admin.Models.Tabs;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models
{
    public class TextWidgetModel : IUpdatePropertiesViewModel<TextWidget>, IAddPropertiesViewModel<TextWidget>
    {
        public string Text { get; set; }
    }
}