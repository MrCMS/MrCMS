using Microsoft.AspNetCore.Mvc;
using MrCMS.Website;
using MrCMS.Website.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.ViewComponents
{
    public class LayoutAreaViewComponent : ViewComponent
    {
        private readonly IFrontEndEditingChecker _frontEndEditingChecker;

        public LayoutAreaViewComponent(IFrontEndEditingChecker frontEndEditingChecker)
        {
            _frontEndEditingChecker = frontEndEditingChecker;
        }

        public async Task<IViewComponentResult> InvokeAsync(string name, bool allowFrontEndEditing = true)
        {
            var info = GetInfo(name);
            if (info == null)
            {
                return Content(string.Empty);
            }

            if (await _frontEndEditingChecker.IsAllowed() && allowFrontEndEditing)
            {
                return View("Editable", info);
            }

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