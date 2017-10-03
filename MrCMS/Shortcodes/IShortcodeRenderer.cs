using System.Collections.Generic;
using MrCMS.Website;

namespace MrCMS.Shortcodes
{
    public interface IShortcodeRenderer
    {
        string TagName { get; }
        string Render(IHtmlHelper helper, Dictionary<string, string> attributes);
    }
}