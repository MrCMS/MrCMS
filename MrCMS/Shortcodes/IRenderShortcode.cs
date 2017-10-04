using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Website;

namespace MrCMS.Shortcodes
{
   

    public interface IRenderShortcode
    {
        bool CanRender(string tagName);
        string Render(IHtmlHelper helper, string tagName, Dictionary<string, string> attributes);
    }

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

        public string Render(IHtmlHelper helper, string tagName, Dictionary<string, string> attributes)
        {
            return !CanRender(tagName)
                ? string.Empty
                : _renderers[tagName].Render(helper, attributes);
        }
    }
}