using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Shortcodes
{
    public class RenderShortcode : IRenderShortcode
    {
        private readonly Dictionary<string, IShortcodeRenderer> _renderers;

        public RenderShortcode(IEnumerable<IShortcodeRenderer> renderers)
        {
            _renderers = renderers.ToDictionary(x => x.TagName, StringComparer.OrdinalIgnoreCase);
        }
        public bool CanRender(string tagName)
        {
            return _renderers.ContainsKey(tagName);
        }

        public async Task<IHtmlContent> Render(IHtmlHelper helper, string tagName, Dictionary<string, string> attributes)
        {
            return !CanRender(tagName)
                ? HtmlString.Empty
                : await _renderers[tagName].Render(helper, attributes);
        }
    }
}