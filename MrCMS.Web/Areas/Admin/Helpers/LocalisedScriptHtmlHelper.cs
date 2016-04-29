using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Website;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class LocalisedScriptHtmlHelper
    {
        public static IHtmlString RenderLocalisedScripts(this HtmlHelper helper)
        {
            var localisedScripts = helper.GetAll<ILocalisedScripts>();
            return new HtmlString(string.Concat(localisedScripts.SelectMany(scripts => scripts.Files.Select(file => $"<script type=\"text/javascript\" src=\"{file}\"></script>"))));
        }
    }
}