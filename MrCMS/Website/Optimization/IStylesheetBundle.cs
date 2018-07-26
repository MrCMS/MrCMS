using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MrCMS.Helpers;

namespace MrCMS.Website.Optimization
{
    public interface IStylesheetBundle
    {
        string Url { get; }
        IEnumerable<string> Files { get; }
    }

    public static class ContentHtmlExtensions
    {
        public static IHtmlContent RenderStyleBundle(this IHtmlHelper helper, string url)
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<IStylesheetBundle>();
            var stylesheetBundles = types.Select(type => helper.ViewContext.HttpContext.RequestServices.GetService(type) as IStylesheetBundle);

            var bundle = stylesheetBundles.FirstOrDefault(x => x.Url.Equals(url, StringComparison.OrdinalIgnoreCase));
            if (bundle != null)
            {
                var builder = new HtmlContentBuilder();

                foreach (var file in bundle.Files)
                {
                    builder.AppendHtmlLine($"<link rel=\"stylesheet\" href=\"{TrimStart(file)}\" />");
                }

                return builder;
            }
            return new HtmlString($"<link rel=\"stylesheet\" href=\"{TrimStart(url)}\" />");
        }

        private static string TrimStart(string url)
        {
            return url?.TrimStart('~');
        }

        public static IHtmlContent RenderScriptBundle(this IHtmlHelper helper, string url)
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<IScriptBundle>();
            var stylesheetBundles = types.Select(type => helper.ViewContext.HttpContext.RequestServices.GetService(type) as IScriptBundle);

            var bundle = stylesheetBundles.FirstOrDefault(x => x.Url.Equals(url, StringComparison.OrdinalIgnoreCase));
            if (bundle != null)
            {
                var builder = new HtmlContentBuilder();

                foreach (var file in bundle.Files)
                {
                    builder.AppendHtmlLine($"<script type=\"text/javascript\" src=\"{TrimStart(file)}\"></script>");
                }
                return builder;
            }
            return new HtmlString($"<script type=\"text/javascript\" src=\"{TrimStart(url)}\"></script>");
        }
    }
}