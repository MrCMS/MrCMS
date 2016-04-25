using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class LocalisedScriptHtmlHelper
    {
        public static IHtmlString RenderLocalisedScripts(this HtmlHelper helper)
            => new HtmlString(string.Concat(GetLocalisedScripts().Select(b => $"<script type=\"text/javascript\" src=\"{b}\" />\n")));

        private static IEnumerable<string> GetLocalisedScripts()
            => new List<string>
                {
                    $"~/Areas/Admin/Content/Scripts/lib/jquery/ui/i18n/jquery.ui.datepicker-{CurrentRequestData.CultureInfo.Name}.js",
                    $"~/Areas/Admin/Content/Scripts/lib/jquery/validate/localization/messages_{CurrentRequestData.CultureInfo.Name.Replace("-", "_")}.js",
                };
    }
}