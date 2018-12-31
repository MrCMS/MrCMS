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

        public IViewComponentResult Invoke(int id, bool editable = false)
        {
            var widget = _widgetUIService.GetModel(id);
            if (widget.Widget == null)
            {
                return Content(string.Empty);
            }
            if (editable)
            {
                return View("Editable", widget.Widget);
            }

            return View(widget.Widget.GetType().Name, widget.Model);
        }
    }
}