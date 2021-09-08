namespace MrCMS.Web.Admin.Helpers
{
    public static class LocalisedScriptHtmlHelper
    {
        // public static void RenderLocalisedScripts(this IHtmlHelper helper)
        // {
        //     var serviceProvider = helper.ViewContext.HttpContext.RequestServices;
        //     var types = TypeHelper.GetAllConcreteTypesAssignableFrom<ILocalisedScripts>();
        //     var localisedScripts = types.Select(serviceProvider.GetService).OfType<ILocalisedScripts>();
        //     var scriptList = localisedScripts.SelectMany(scripts => scripts.Files).ToArray();
        //     helper.WriteScriptsToResponse(scriptList);
        // }
    }
}