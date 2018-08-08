using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
}