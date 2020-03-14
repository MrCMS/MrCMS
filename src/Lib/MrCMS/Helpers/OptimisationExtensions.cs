using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;
using MrCMS.Website.Optimization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Helpers
{
    public static class OptimisationExtensions
    {
        public static void IncludeScript(this IHtmlHelper helper, string url)
        {
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>()
                .AddScript(helper, url);
        }

        public static async Task RenderScripts(this IHtmlHelper helper)
        {
            await helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>()
                .GetScripts(helper.ViewContext);
        }

        public static async Task WriteScriptsToResponse(this IHtmlHelper helper, IEnumerable<string> urls)
        {
            await helper.ViewContext.WriteScriptsToResponse(urls);
        }

        internal static async Task WriteScriptsToResponse(this ViewContext viewContext, IEnumerable<string> urls)
        {
            foreach (string path in urls)
            {
                await viewContext.Writer.WriteAsync(
                    $"<script src=\"{(path.StartsWith("~") ? path.Substring(1) : path)}\" type=\"text/javascript\"></script>");
            }
        }

        public static async Task WriteCssToResponse(this IHtmlHelper helper, IEnumerable<string> urls)
        {
            await helper.ViewContext.WriteCssToResponse(urls);
        }
        internal static async Task WriteCssToResponse(this ViewContext viewContext, IEnumerable<string> urls)
        {
            foreach (string path in urls)
            {
                await viewContext.Writer.WriteAsync(
                                   $"<link href=\"{(path.StartsWith("~") ? path.Substring(1) : path)}\" rel=\"stylesheet\" type=\"text/css\" />");
            }
        }

        public static void IncludeCss(this IHtmlHelper helper, string url)
        {
            helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>().AddCss(helper, url);
        }

        public static async Task RenderCss(this IHtmlHelper helper)
        {
            await helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IResourceBundler>()
                .GetCss(helper.ViewContext);
            //MrCMSApplication.Get<IResourceBundler>().GetCss(helper.ViewContext);
        }

        public static void AddAppUIScripts(this IHtmlHelper html)
        {
            foreach (var script in html.ViewContext.HttpContext.RequestServices.GetServices<IAppScriptList>()
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