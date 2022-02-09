using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Shortcodes
{
    public interface IShortcodeParser
    {
        IHtmlContent Parse(IHtmlHelper htmlHelper, string current);
    }
}