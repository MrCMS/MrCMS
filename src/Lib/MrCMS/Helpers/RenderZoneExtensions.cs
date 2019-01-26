using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.ViewComponents;

namespace MrCMS.Helpers
{
    public static class RenderZoneExtensions
    {
        public static Task<IHtmlContent> RenderZone(this IViewComponentHelper helper, string name, bool allowFrontEndEditing = true)
        {
            return helper.InvokeAsync<LayoutAreaViewComponent>(new { name, allowFrontEndEditing });
        }
    }
}