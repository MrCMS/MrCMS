using System.Web.Mvc;

namespace MrCMS.Shortcodes
{
    public interface IShortcodeParser
    {
        string Parse(HtmlHelper htmlHelper, string current);
    }
}