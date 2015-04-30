using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using StackExchange.Profiling;

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
            return _widgetUIService.GetContent(this, widget, helper => helper.Action("Internal", "Widget", new {widget}));
        }

        public PartialViewResult Internal(Widget widget)
        {
            _widgetUIService.SetAppDataToken(RouteData, widget);

            return PartialView(!string.IsNullOrWhiteSpace(widget.CustomLayout)
                ? widget.CustomLayout
                : widget.WidgetType,
                _widgetUIService.GetModel(widget));
        }

        protected override IDisposable GetEndToEndStep(ActionExecutingContext filterContext)
        {
            var widget = filterContext.ActionParameters["widget"] as Widget;
            if (widget == null)
                return base.GetEndToEndStep(filterContext);
            return MiniProfiler.Current.Step(string.Format("End-to-end for widget {0} ({1})",
                widget.Id, widget.Name));
        }
    }
}