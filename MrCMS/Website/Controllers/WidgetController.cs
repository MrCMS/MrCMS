using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    public class WidgetController : MrCMSUIController
    {
        private readonly IWidgetService _widgetService;

        public WidgetController(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        public PartialViewResult Show(Widget widget)
        {
            return PartialView(!string.IsNullOrWhiteSpace(widget.CustomLayout) ? widget.CustomLayout : widget.WidgetType, _widgetService.GetModel(widget));
        }
    }
}