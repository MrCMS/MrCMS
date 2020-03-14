using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Website;

namespace MrCMS.Shortcodes
{
    public interface IRenderShortcode
    {
        bool CanRender(string tagName);
        Task<IHtmlContent> Render(IHtmlHelper helper, string tagName, Dictionary<string, string> attributes);
    }
}