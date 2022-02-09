using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Shortcodes;

namespace MrCMS.Helpers
{
    public static class ShortcodeParsingExtensions
    {
        public static IHtmlContent ParseShortcodes(this IHtmlHelper htmlHelper, string content)
        {
            return htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IShortcodeParser>()
                .Parse(htmlHelper, content);
        }
    }
}