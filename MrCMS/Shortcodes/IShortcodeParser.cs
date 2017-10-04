using System.Web.Mvc;
using MrCMS.Website;

namespace MrCMS.Shortcodes
{
    public interface IShortcodeParser
    {
        MvcHtmlString Parse(IHtmlHelper htmlHelper, string current);
    }
}