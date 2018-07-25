using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.ViewComponents
{
    public class LayoutAreaViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string name, bool allowFrontEndEditing = false)
        {
            var info = GetInfo(name);
            if (info == null)
                return Content(string.Empty);
            return View(info);
        }

        private WidgetDisplayInfo GetInfo(string areaName)
        {
            var infos = ViewData[ProcessWebpageViews.WidgetData] as IDictionary<string, WidgetDisplayInfo>;

            return infos?.ContainsKey(areaName) != true
                ? null
                : infos[areaName];
        }
    }
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