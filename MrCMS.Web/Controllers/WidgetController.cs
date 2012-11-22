using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class WidgetController : MrCMSController
    {
        private readonly IWidgetService _widgetService;

        public WidgetController(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        public PartialViewResult Show(int id)
        {
            var widget = _widgetService.GetWidget<Widget>(id);

            return PartialView(!string.IsNullOrWhiteSpace(widget.CustomLayout) ? widget.CustomLayout : widget.WidgetType, _widgetService.GetModel(widget));
        }
    }
}