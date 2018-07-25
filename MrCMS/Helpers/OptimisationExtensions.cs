using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;
using MrCMS.Website.Optimization;

namespace MrCMS.Helpers
{
    public static class OptimisationExtensions
    {
        public static void IncludeScript(this IHtmlHelper helper, string url)
        {
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>().AddScript(helper, url);
        }

        public static void RenderScripts(this IHtmlHelper helper)
        {
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>()
                .GetScripts(helper.ViewContext);
        }

        public static void IncludeCss(this IHtmlHelper helper, string url)
        {
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>().AddCss(helper, url);
        }

        public static void RenderCss(this IHtmlHelper helper)
        {
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>()
                .GetCss(helper.ViewContext);
            //MrCMSApplication.Get<IResourceBundler>().GetCss(helper.ViewContext);
        }

        public static void AddAppUIScripts(this IHtmlHelper html)
        {
            foreach (var script in html.ViewContext.HttpContext.RequestServices.GetServices<IAppScriptList>()
                .SelectMany(appScriptList => appScriptList.UIScripts)) html.IncludeScript(script);
        }

        public static void AddAppAdminScripts(this IHtmlHelper html)
        {
            foreach (var script in html.ViewContext.HttpContext.RequestServices.GetServices<IAppScriptList>()
                .SelectMany(appScriptList => appScriptList.AdminScripts)) html.IncludeScript(script);
        }

        public static void AddAppUIStylesheets(this IHtmlHelper html)
        {
            foreach (var script in html.ViewContext.HttpContext.RequestServices.GetServices<IAppStylesheetList>()
                .SelectMany(appScriptList => appScriptList.UIStylesheets)) html.IncludeCss(script);
        }

        public static void AddAppAdminStylesheets(this IHtmlHelper html)
        {
            foreach (var script in html.ViewContext.HttpContext.RequestServices.GetServices<IAppStylesheetList>()
                .SelectMany(appScriptList => appScriptList.AdminStylesheets)) html.IncludeCss(script);
        }
    }
}