using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Shortcodes;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class ShortcodeParsingExtensions
    {
        public static IHtmlString ParseShortcodes(this HtmlHelper htmlHelper, string content)
        {
            var shortcodeParsers = MrCMSApplication.GetAll<IShortcodeParser>();

            content = shortcodeParsers.Aggregate(content, (current, shortcodeParser) => shortcodeParser.Parse(htmlHelper, current));

            return new HtmlString(content);
        }
    }
}