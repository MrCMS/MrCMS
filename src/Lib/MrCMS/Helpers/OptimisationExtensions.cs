using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;
using MrCMS.Website.Optimization;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Helpers
{
    public static class OptimisationExtensions
    {
        public static void IncludeScript(this IHtmlHelper helper, string url)
        {
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>()
                .AddScript(helper, url);
        }

        public static void RenderScripts(this IHtmlHelper helper)
        {
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>()
                .GetScripts(helper.ViewContext);
        }

        public static void WriteScriptsToResponse(this IHtmlHelper helper, IEnumerable<string> urls)
        {
            helper.ViewContext.WriteScriptsToResponse(urls);
        }

        internal static void WriteScriptsToResponse(this ViewContext viewContext, IEnumerable<string> urls)
        {
            foreach (string path in urls)
            {
                viewContext.Writer.Write("<script src=\"{0}\" type=\"text/javascript\"></script>",
                    path.StartsWith("~") ? path.Substring(1) : path);
            }
        }

        public static void WriteCssToResponse(this IHtmlHelper helper, IEnumerable<string> urls)
        {
            helper.ViewContext.WriteCssToResponse(urls);
        }
        internal static void WriteCssToResponse(this ViewContext viewContext, IEnumerable<string> urls)
        {
            foreach (string path in urls)
            {
                viewContext.Writer.Write("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                    path.StartsWith("~") ? path.Substring(1) : path);
            }
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
            var scriptLists = html.ViewContext.HttpContext.RequestServices.GetServices<IAppScriptList>().ToList();
            foreach (var script in scriptLists
                .SelectMany(appScriptList => appScriptList.UIScripts))
            {
                html.IncludeScript(script);
            }
        }

        public static void AddAppAdminScripts(this IHtmlHelper html)
        {
            foreach (var script in html.ViewContext.HttpContext.RequestServices.GetServices<IAppScriptList>()
                .SelectMany(appScriptList => appScriptList.AdminScripts))
            {
                html.IncludeScript(script);
            }
        }

        public static void AddAppUIStylesheets(this IHtmlHelper html)
        {
            foreach (var script in html.ViewContext.HttpContext.RequestServices.GetServices<IAppStylesheetList>()
                .SelectMany(appScriptList => appScriptList.UIStylesheets))
            {
                html.IncludeCss(script);
            }
        }

        public static void AddAppAdminStylesheets(this IHtmlHelper html)
        {
            foreach (var script in html.ViewContext.HttpContext.RequestServices.GetServices<IAppStylesheetList>()
                .SelectMany(appScriptList => appScriptList.AdminStylesheets))
            {
                html.IncludeCss(script);
            }
        }
    }
}