using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Widget;
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

        public async Task<IViewComponentResult> InvokeAsync(int id, bool editable = false)
        {
            var widget = await _widgetUIService.GetWidget(id);
            if (widget == null)
            {
                return Content(string.Empty);
            }
            var model = await _widgetUIService.GetModel(widget);
            //if (editable)
            //{
            //    return View("Editable", widget);
            //}

            return View(widget.GetType().Name, model);
        }
    }
}