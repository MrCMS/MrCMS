using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Website;
using MrCMS.Website.Auth;

namespace MrCMS.Helpers
{
    public static class RenderZoneExtensions
    {
        public static async Task RenderZone(this IHtmlHelper helper, string name)
        {
            var info = GetInfo(helper.ViewData, name);
            if (info == null)
                return;
            var isAllowed = await helper.GetRequiredService<IFrontEndEditingChecker>().IsAllowed();
            var partialViewName = isAllowed ? "_LayoutAreaEditable" : "_LayoutArea";
            await helper.RenderPartialAsync(partialViewName, info);
            // return helper.InvokeAsync<LayoutAreaViewComponent>(new { name, allowFrontEndEditing });
        }

        private static WidgetDisplayInfo GetInfo(ViewDataDictionary viewData, string areaName)
        {
            var infos = viewData[ProcessWebpageViews.WidgetData] as IDictionary<string, WidgetDisplayInfo>;

            return infos?.ContainsKey(areaName) != true
                ? null
                : infos[areaName];
        }
    }
}