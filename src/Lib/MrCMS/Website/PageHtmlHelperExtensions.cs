using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ACL.Rules;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Auth;

namespace MrCMS.Website
{
    public static class PageHtmlHelperExtensions
    {
        public static async Task<IHtmlContent> Editable<T>(this IHtmlHelper<T> helper,
            Expression<Func<T, string>> method,
            bool isHtml = false) where T : SystemEntity
        {
            var model = helper.ViewData.Model;
            return model == null
                ? HtmlString.Empty
                : await helper.Editable(model, method, isHtml);
        }


        public static async Task<bool> EditingEnabled(this IHtmlHelper helper)
        {
            var serviceProvider = helper.ViewContext.HttpContext.RequestServices;
            var currentUser = await serviceProvider.GetRequiredService<IGetCurrentUser>().Get();
            var accessChecker = serviceProvider.GetRequiredService<IAccessChecker>();
            return currentUser != null && await accessChecker.CanAccess<AdminBarACL>(AdminBarACL.Show, currentUser) &&
                   serviceProvider.GetRequiredService<SiteSettings>().EnableInlineEditing;
        }

        public static async Task<IHtmlContent> Editable<T>(this IHtmlHelper helper, T model,
            Expression<Func<T, string>> method,
            bool isHtml = false) where T : SystemEntity
        {
            if (model == null)
                return HtmlString.Empty;

            var propertyInfo = PropertyFinder.GetProperty(method);

            var content = method.Compile().Invoke(model);

            if (!string.IsNullOrWhiteSpace(content) && content.Contains("[page-break]"))
            {
                var pageNumber = GetPageNumber(helper.ViewContext.HttpContext.Request);
                var pages = helper.GetRequiredService<IGetPagesFromContent>().Get(content);
                if (pages.Any())
                    content = pages.Count < pageNumber ? pages.First() : pages[pageNumber - 1];
            }

            var value = helper.ParseShortcodes(content);

            var typeName = model.GetType().Name;

            if (await helper.EditingEnabled() && propertyInfo != null)
            {
                var tagBuilder = new TagBuilder(isHtml ? "div" : "span");
                tagBuilder.AddCssClass("editable");
                tagBuilder.Attributes["data-id"] = model.Id.ToString();
                tagBuilder.Attributes["data-property"] = propertyInfo.Name;
                tagBuilder.Attributes["data-type"] = typeName;
                tagBuilder.Attributes["data-is-html"] = isHtml ? "true" : "false";
                tagBuilder.InnerHtml.AppendHtml(value);

                return tagBuilder;
            }

            return value;
        }


        private static int GetPageNumber(HttpRequest request)
        {
            var p = request.Query["p"];
            if (int.TryParse(p, out var pageNum))
                return pageNum;

            return 1;
        }
    }
}