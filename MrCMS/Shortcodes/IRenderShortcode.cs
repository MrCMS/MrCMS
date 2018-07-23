using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Website;

namespace MrCMS.Shortcodes
{
   

    public interface IRenderShortcode
    {
        bool CanRender(string tagName);
        string Render(IHtmlHelper helper, string tagName, Dictionary<string, string> attributes);
    }
}