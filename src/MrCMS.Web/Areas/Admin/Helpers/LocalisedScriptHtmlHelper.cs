using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class LocalisedScriptHtmlHelper
    {
        public static async Task RenderLocalisedScripts(this IHtmlHelper helper)
        {
            var serviceProvider = helper.ViewContext.HttpContext.RequestServices;
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<ILocalisedScripts>();
            var localisedScripts = types.Select(serviceProvider.GetService).OfType<ILocalisedScripts>();
            var scriptList= new List<string>();
            foreach (var script in localisedScripts)
            {
                scriptList.AddRange(await script.GetFiles());
            }
            await helper.WriteScriptsToResponse(scriptList);
        }
    }
}