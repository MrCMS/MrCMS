using MrCMS.Web.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Widgets
{
    public class PlainTextWidgetModel : IUpdatePropertiesViewModel<PlainTextWidget>, IAddPropertiesViewModel<PlainTextWidget>
    {
        public string Text { get; set; }
    }
}