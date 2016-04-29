using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class LocalisedScriptHtmlHelper
    {
        public static IHtmlString RenderLocalisedScripts(this HtmlHelper helper)
        {
            var localisedScripts = helper.GetAll<ILocalisedScripts>();
            var scriptList = localisedScripts.SelectMany(scripts => scripts.Files.Select(GetScriptTag));
            return MvcHtmlString.Create(string.Join(string.Empty, scriptList));
        }

        private static string GetScriptTag(string file)
        {
            return string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", file);
        }
    }
}