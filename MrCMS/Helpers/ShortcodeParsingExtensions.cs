using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Shortcodes;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class ShortcodeParsingExtensions
    {
        public static IHtmlString ParseShortcodes(this IHtmlHelper htmlHelper, string content)
        {
            return htmlHelper.Get<IShortcodeParser>().Parse(htmlHelper, content);
        }
    }
}