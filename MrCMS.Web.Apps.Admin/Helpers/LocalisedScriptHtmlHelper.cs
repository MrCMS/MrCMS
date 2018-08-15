using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Services;
using System.Linq;

namespace MrCMS.Web.Apps.Admin.Helpers
{
    public static class LocalisedScriptHtmlHelper
    {
        public static void RenderLocalisedScripts(this IHtmlHelper helper)
        {
            var serviceProvider = helper.ViewContext.HttpContext.RequestServices;
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<ILocalisedScripts>();
            var localisedScripts = types.Select(serviceProvider.GetService).OfType<ILocalisedScripts>();
            var scriptList = localisedScripts.SelectMany(scripts => scripts.Files).ToArray();
            helper.WriteScriptsToResponse(scriptList);
        }
    }
}