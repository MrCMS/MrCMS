using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Web.Apps.Admin.Services;

namespace MrCMS.Web.Apps.Admin.Helpers
{
    public static class LocalisedScriptHtmlHelper
    {
        public static void RenderLocalisedScripts(this IHtmlHelper helper)
        {
            var localisedScripts = helper.ViewContext.HttpContext.RequestServices.GetServices<ILocalisedScripts>();
            var scriptList = localisedScripts.SelectMany(scripts => scripts.Files).ToArray();
            //helper.ViewContext.Writer.Write(Scripts.Render(scriptList));
            // TODO: write scripts to view context 
        }
    }
}