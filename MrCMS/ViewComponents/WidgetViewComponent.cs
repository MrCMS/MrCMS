using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;

namespace MrCMS.ViewComponents
{
    public class WidgetViewComponent : ViewComponent
    {
        private readonly IWidgetUIService _widgetUIService;

        public WidgetViewComponent(IWidgetUIService widgetUIService)
        {
            _widgetUIService = widgetUIService;
        }

        public IViewComponentResult Invoke(int id)
        {
            var widget = _widgetUIService.GetModel(id);
            if (widget.Widget == null)
                return Content(string.Empty);
            return View(widget.Widget.GetType().Name, widget.Model);
        }
    }
}