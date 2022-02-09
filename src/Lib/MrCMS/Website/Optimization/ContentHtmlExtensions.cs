using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Website.Optimization
{
    public static class ContentHtmlExtensions
    {
        private static Guid RuntimeKey = Guid.NewGuid();

        private static readonly HashSet<Type> UIScriptBundleTypes =
            TypeHelper.GetAllConcreteTypesAssignableFrom<IUIScriptBundle>();

        private static readonly HashSet<Type> UIStyleBundleTypes =
            TypeHelper.GetAllConcreteTypesAssignableFrom<IUIStyleBundle>();

        private static readonly HashSet<Type> AdminScriptBundleTypes =
            TypeHelper.GetAllConcreteTypesAssignableFrom<IAdminScriptBundle>();

        private static readonly HashSet<Type> AdminStyleBundleTypes =
            TypeHelper.GetAllConcreteTypesAssignableFrom<IAdminStyleBundle>();

        public static async Task<IHtmlContent> RenderUIVendorScripts(this IHtmlHelper helper)
        {
            var theme = helper.GetRequiredService<SiteSettings>().ThemeName;
            var bundles = UIScriptBundleTypes.Select(type =>
                    helper.ViewContext.HttpContext.RequestServices.GetService(type) as IUIScriptBundle)
                .OrderByDescending(x => x.Priority);

            var files = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                if (await bundle.ShouldShow(theme))
                    files.AddRange(bundle.VendorFiles);
            }

            return RenderScriptList(files);
        }

        public static async Task<IHtmlContent> RenderUIVendorStyles(this IHtmlHelper helper)
        {
            var theme = helper.GetRequiredService<SiteSettings>().ThemeName;
            var bundles = UIStyleBundleTypes.Select(type =>
                    helper.ViewContext.HttpContext.RequestServices.GetService(type) as IUIStyleBundle)
                .OrderByDescending(x => x.Priority);

            var files = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                if (await bundle.ShouldShow(theme))
                    files.AddRange(bundle.VendorFiles);
            }

            return RenderStyleList(files);
        }

        public static async Task<IHtmlContent> RenderProdUIScripts(this IHtmlHelper helper)
        {
            async Task<IHtmlContent> GetDefaultOutput()
            {
                IHtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
                htmlContentBuilder = htmlContentBuilder.AppendHtml(await helper.RenderUIVendorScripts());
                htmlContentBuilder = htmlContentBuilder.AppendHtml(await helper.RenderUIScripts());
                return htmlContentBuilder;
            }

            var themeName = helper.GetRequiredService<SiteSettings>().ThemeName;
            // no theme means no theme to render
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return await GetDefaultOutput();
            }

            var context = helper.GetRequiredService<MrCMSAppContext>();
            var theme = context.Themes.FirstOrDefault(x => x.Name == themeName);
            // no theme means no theme to render
            if (theme == null)
            {
                return await GetDefaultOutput();
            }

            return RenderScriptList(new HashSet<string> {$"/{theme.OutputPrefix}.js"});
        }
        public static async Task<IHtmlContent> RenderUIScripts(this IHtmlHelper helper)
        {
            var theme = helper.GetRequiredService<SiteSettings>().ThemeName;
            var bundles = UIScriptBundleTypes.Select(type =>
                    helper.ViewContext.HttpContext.RequestServices.GetService(type) as IUIScriptBundle)
                .OrderByDescending(x => x.Priority);

            var files = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                if (!await bundle.ShouldShow(theme))
                    continue;
                var url = bundle.Url;
                if (url != null)
                    files.Add(url);
            }

            return RenderScriptList(files);
        }

        public static async Task<IHtmlContent> RenderProdUIStyles(this IHtmlHelper helper)
        {
            async Task<IHtmlContent> GetDefaultOutput()
            {
                IHtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
                htmlContentBuilder = htmlContentBuilder.AppendHtml(await helper.RenderUIVendorStyles());
                htmlContentBuilder = htmlContentBuilder.AppendHtml(await helper.RenderUIStyles());
                return htmlContentBuilder;
            }

            var themeName = helper.GetRequiredService<SiteSettings>().ThemeName;
            // no theme means no theme to render
            if (string.IsNullOrWhiteSpace(themeName))
            {
                return await GetDefaultOutput();
            }

            var context = helper.GetRequiredService<MrCMSAppContext>();
            var theme = context.Themes.FirstOrDefault(x => x.Name == themeName);
            // no theme means no theme to render
            if (theme == null)
            {
                return await GetDefaultOutput();
            }

            return RenderStyleList(new HashSet<string> {$"/{theme.OutputPrefix}.css"});
        }
        public static async Task<IHtmlContent> RenderUIStyles(this IHtmlHelper helper)
        {
            var theme = helper.GetRequiredService<SiteSettings>().ThemeName;
            var bundles = UIStyleBundleTypes.Select(type =>
                    helper.ViewContext.HttpContext.RequestServices.GetService(type) ).OfType<IUIStyleBundle>()
                .OrderByDescending(x => x.Priority);

            var files = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                if (!await bundle.ShouldShow(theme))
                    continue;
                var url = bundle.Url;
                if (url != null)
                    files.Add(url);
            }

            return RenderStyleList(files);
        }

        public static async Task<IHtmlContent> RenderAdminVendorScripts(this IHtmlHelper helper)
        {
            var theme = helper.GetRequiredService<SiteSettings>().ThemeName;
            var bundles = AdminScriptBundleTypes.Select(type =>
                    helper.ViewContext.HttpContext.RequestServices.GetService(type) as IAdminScriptBundle)
                .OrderByDescending(x => x.Priority);
            
            var files = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                if (await bundle.ShouldShow(theme))
                    files.AddRange(bundle.VendorFiles);
            }

            return RenderScriptList(files);
        }

        public static async Task<IHtmlContent> RenderAdminVendorStyles(this IHtmlHelper helper)
        {
            var theme = helper.GetRequiredService<SiteSettings>().ThemeName;
            var bundles = AdminStyleBundleTypes.Select(type =>
                    helper.ViewContext.HttpContext.RequestServices.GetService(type) as IAdminStyleBundle)
                .OrderByDescending(x => x.Priority);

            var files = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                if (await bundle.ShouldShow(theme))
                    files.AddRange(bundle.VendorFiles);
            }

            return RenderStyleList(files);
        }

        public static async Task<IHtmlContent> RenderAdminScripts(this IHtmlHelper helper)
        {
            var theme = helper.GetRequiredService<SiteSettings>().ThemeName;
            var bundles = AdminScriptBundleTypes.Select(type =>
                    helper.ViewContext.HttpContext.RequestServices.GetService(type) as IAdminScriptBundle)
                .OrderByDescending(x => x.Priority);
            
            var files = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                if (!await bundle.ShouldShow(theme))
                    continue;
                var url = bundle.Url;
                if (url != null)
                    files.Add(url);
            }

            return RenderScriptList(files);
        }

        public static async Task<IHtmlContent> RenderAdminStyles(this IHtmlHelper helper)
        {
            var theme = helper.GetRequiredService<SiteSettings>().ThemeName;
            var bundles = AdminStyleBundleTypes.Select(type =>
                    helper.ViewContext.HttpContext.RequestServices.GetService(type) as IAdminStyleBundle)
                .OrderByDescending(x => x.Priority);

            var files = new HashSet<string>();
            foreach (var bundle in bundles)
            {
                if (!await bundle.ShouldShow(theme))
                    continue;
                var url = bundle.Url;
                if (url != null)
                    files.Add(url);
            }

            return RenderStyleList(files);
        }

        private static IHtmlContent RenderScriptList(HashSet<string> files)
        {
            if (!files.Any()) return HtmlString.Empty;
            var builder = new HtmlContentBuilder();

            foreach (var file in files)
            {
                builder.AppendHtmlLine($"<script type=\"text/javascript\" src=\"{GetUrl(file)}\"></script>");
            }

            return builder;
        }

        private static string GetUrl(string file)
        {
            // if it's public, we don't manage changes
            if (file.StartsWith("http"))
                return file;
            // else append a key
            return file + $"?v={RuntimeKey}";
        }

        private static IHtmlContent RenderStyleList(HashSet<string> files)
        {
            if (!files.Any()) return HtmlString.Empty;
            var builder = new HtmlContentBuilder();

            foreach (var file in files)
            {
                builder.AppendHtmlLine($"<link rel=\"stylesheet\" href=\"{GetUrl(file)}\" />");
            }

            return builder;
        }

    }
}