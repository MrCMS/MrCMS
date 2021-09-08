using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Shortcodes
{
    public interface IShortcodeRenderer
    {
        string TagName { get; }
        IHtmlContent Render(IHtmlHelper helper, Dictionary<string, string> attributes);
    }
}