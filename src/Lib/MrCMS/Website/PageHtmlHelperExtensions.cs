using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
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
        public static IHtmlContent Editable<T>(this IHtmlHelper<T> helper, Expression<Func<T, string>> method, bool isHtml = false) where T : SystemEntity
        {
            var model = helper.ViewData.Model;
            if (model == null)
                return HtmlString.Empty;

            var propertyInfo = PropertyFinder.GetProperty(method);

            var value = helper.ParseShortcodes(method.Compile().Invoke(model));

            var typeName = model.GetType().Name;

            if (helper.EditingEnabled() && propertyInfo != null)
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
        public static bool EditingEnabled(this IHtmlHelper helper)
        {
            var serviceProvider = helper.ViewContext.HttpContext.RequestServices;
            var currentUser = serviceProvider.GetRequiredService<IGetCurrentUser>() .Get();
            var accessChecker = serviceProvider.GetRequiredService<IAccessChecker>();
            return currentUser != null && accessChecker.CanAccess<AdminBarACL>(AdminBarACL.Show) &&
                   serviceProvider.GetRequiredService<SiteSettings>().EnableInlineEditing;
        }

    }
}