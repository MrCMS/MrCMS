using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Widgets
{
    public class TextWidgetModel : IUpdatePropertiesViewModel<TextWidget>, IAddPropertiesViewModel<TextWidget>
    {
        public string Text { get; set; }
    }
}