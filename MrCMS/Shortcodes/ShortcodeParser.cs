using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MrCMS.Website;

namespace MrCMS.Shortcodes
{
    public class ShortcodeParser : IShortcodeParser
    {
        private readonly IRenderShortcode _renderShortcode;

        public ShortcodeParser(IRenderShortcode renderShortcode)
        {
            _renderShortcode = renderShortcode;
        }

        public MvcHtmlString Parse(IHtmlHelper htmlHelper, string current)
        {
            if (string.IsNullOrWhiteSpace(current))
                return MvcHtmlString.Empty;

            current = ShortcodeMatcher.Replace(current, match =>
            {
                var tagName = match.Groups[1].Value;
                if (!_renderShortcode.CanRender(tagName))
                    return string.Empty;

                var matches = AttributeMatcher.Matches(match.Groups[2].Value);

                var attributes = matches.Cast<Match>().ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);

                return _renderShortcode.Render(htmlHelper, tagName, attributes);
            });

            return MvcHtmlString.Create(current);
        }

        private static readonly Regex ShortcodeMatcher =
            new Regex(@"\[([\w-_]+)([^\]]*)?\]?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex AttributeMatcher =
            new Regex(@"(\w+)\s*=\s*""?(\d+)""?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}