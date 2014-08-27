using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Apps;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Helpers;

namespace MrCMS.Website.Controllers
{
    public class WidgetController : MrCMSUIController
    {
        private readonly IWidgetUIService _widgetUIService;

        public WidgetController(IWidgetUIService widgetUIService)
        {
            _widgetUIService = widgetUIService;
        }

        public ActionResult Show(Widget widget)
        {
            return _widgetUIService.GetContent(this, widget, helper => helper.Action("Internal", "Widget", new { widget }));
        }

        public PartialViewResult Internal(Widget widget)
        {
            _widgetUIService.SetAppDataToken(RouteData, widget);

            return PartialView(!string.IsNullOrWhiteSpace(widget.CustomLayout)
                ? widget.CustomLayout
                : widget.WidgetType,
                _widgetUIService.GetModel(widget));
        }
    }
}