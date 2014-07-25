using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Helpers;

namespace MrCMS.Website.Controllers
{
    public class WidgetController : MrCMSUIController
    {
        private readonly IWidgetModelService _service;

        public WidgetController(IWidgetModelService service)
        {
            _service = service;
        }

        public PartialViewResult Show(Widget widget)
        {
            if (MrCMSApp.AppWidgets.ContainsKey(widget.Unproxy().GetType()))
                RouteData.DataTokens["app"] = MrCMSApp.AppWidgets[widget.Unproxy().GetType()];
            return PartialView(
                !string.IsNullOrWhiteSpace(widget.CustomLayout) ? widget.CustomLayout : widget.WidgetType,
                _service.GetModel(widget));
        }
    }
}