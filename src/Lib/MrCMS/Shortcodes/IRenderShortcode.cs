using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Shortcodes
{
    public interface IRenderShortcode
    {
        bool CanRender(string tagName);
        IHtmlContent Render(IHtmlHelper helper, string tagName, Dictionary<string, string> attributes);
    }
}