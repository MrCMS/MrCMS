using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Shortcodes;
using MrCMS.Website;
using MrCMS.Website.Optimization;
using Newtonsoft.Json;

namespace MrCMS.Helpers
{
    public static class MrCMSHtmlHelper
    {
        private static MvcHtmlString CheckBoxHelper<TModel>(HtmlHelper<TModel> htmlHelper, ModelMetadata metadata,
                                                            string name, bool? isChecked,
                                                            IDictionary<string, object> htmlAttributes)
        {
            bool explicitValue = isChecked.HasValue;
            if (explicitValue)
                htmlAttributes.Remove("checked"); // Explicit value must override dictionary

            return MakeCheckbox(htmlHelper, InputType.CheckBox, metadata, name, !explicitValue /* useViewData */,
                               isChecked ?? false, htmlAttributes);
        }

        private static MvcHtmlString MakeCheckbox(HtmlHelper htmlHelper, InputType inputType, ModelMetadata metadata,
                                                 string name, bool useViewData, bool isChecked, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(inputType));
            tagBuilder.MergeAttribute("name", fullName, true);

            string valueParameter = Convert.ToString("true", CultureInfo.CurrentCulture);
            bool usedModelState = false;

            var modelStateWasChecked = htmlHelper.GetModelStateValue(fullName, typeof(bool)) as bool?;
            if (modelStateWasChecked.HasValue)
            {
                isChecked = modelStateWasChecked.Value;
                usedModelState = true;
            }
            if (!usedModelState)
            {
                var modelStateValue = htmlHelper.GetModelStateValue(fullName, typeof(string)) as string;
                if (modelStateValue != null)
                {
                    isChecked = String.Equals(modelStateValue, valueParameter, StringComparison.Ordinal);
                    usedModelState = true;
                }
            }
            if (!usedModelState && useViewData)
                isChecked = htmlHelper.EvalBoolean(fullName);

            if (isChecked)
                tagBuilder.MergeAttribute("checked", "checked");

            tagBuilder.MergeAttribute("value", valueParameter, false);

            tagBuilder.GenerateId(fullName);

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            if (inputType == InputType.CheckBox)
            {
                // Render an additional <input type="hidden".../> for checkboxes. This 
                // addresses scenarios where unchecked checkboxes are not sent in the request.
                // Sending a hidden input makes it possible to know that the checkbox was present 
                // on the page when the request was submitted. 
                var inputItemBuilder = new StringBuilder();
                inputItemBuilder.Append(tagBuilder.ToString(TagRenderMode.SelfClosing));

                var hiddenInput = new TagBuilder("input");
                hiddenInput.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
                hiddenInput.MergeAttribute("name", fullName);
                hiddenInput.MergeAttribute("value", "false");
                inputItemBuilder.Append(hiddenInput.ToString(TagRenderMode.SelfClosing));
                return MvcHtmlString.Create(inputItemBuilder.ToString());
            }

            return tagBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
        }

        internal static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName,
                                                  IDictionary<string, object> htmlAttributes, string labelText = null)
        {
            string resolvedLabelText = labelText ??
                                       metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(resolvedLabelText))
                return MvcHtmlString.Empty;

            var tag = new TagBuilder("label");
            tag.Attributes.Add("for",
                               TagBuilder.CreateSanitizedId(
                                   html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));

            if (htmlAttributes != null)
                tag.MergeAttributes(htmlAttributes);

            tag.InnerHtml = resolvedLabelText;
            return tag.ToMvcHtmlString(TagRenderMode.Normal);
        }

        private static object GetModelStateValue(this HtmlHelper html, string key, Type destinationType)
        {
            ModelState modelState;
            if (html.ViewData.ModelState.TryGetValue(key, out modelState) && modelState.Value != null)
                return modelState.Value.ConvertTo(destinationType, null /* culture */);
            return null;
        }

        private static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
        {
            return new MvcHtmlString(tagBuilder.ToString(renderMode));
        }

        private static string EvalString(this HtmlHelper html, string key)
        {
            return Convert.ToString(html.ViewData.Eval(key), CultureInfo.CurrentCulture);
        }

        private static bool EvalBoolean(this HtmlHelper html, string key)
        {
            return Convert.ToBoolean(html.ViewData.Eval(key), CultureInfo.InvariantCulture);
        }

        public static MvcHtmlString FormLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                             string controllerName, object routeValues, object htmlAttributes)
        {
            var routeValuesDictionary = new RouteValueDictionary(routeValues);
            var htmlAttributeDictionary = new RouteValueDictionary(htmlAttributes);

            htmlAttributeDictionary["data-action"] = "post-link";
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValuesDictionary,
                                         htmlAttributeDictionary);
        }

        public static MvcHtmlString AjaxFormLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                                 string controllerName, object routeValues, object htmlAttributes)
        {
            var routeValuesDictionary = new RouteValueDictionary(routeValues);
            IDictionary<string, object> htmlAttributeDictionary = AnonymousObjectToHtmlAttributes(htmlAttributes);
            htmlAttributeDictionary.Add("data-action", "post-link-ajax");
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValuesDictionary,
                                         htmlAttributeDictionary);
        }

        public static MvcHtmlString AjaxFormLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                                 object routeValues, object htmlAttributes)
        {
            return AjaxFormLink(htmlHelper, linkText, actionName, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString FormLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                             object routeValues, object htmlAttributes)
        {
            return FormLink(htmlHelper, linkText, actionName, null, routeValues, htmlAttributes);
        }


        public static MvcHtmlString LabelFor<TModel>(this HtmlHelper<TModel> htmlHelper,
                                                     Expression<Func<TModel, object>> expression, object htmlAttributes,
                                                     string text = null)
        {
            return LabelHelper(htmlHelper, ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData),
                               ExpressionHelper.GetExpressionText(expression), AnonymousObjectToHtmlAttributes(htmlAttributes),
                               text);
        }

        public static MvcHtmlString InlineCheckboxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
                                                     Expression<Func<TModel, bool>> expression, object labelAttributes = null, object checkboxAttributes = null,
                                                     string text = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var checkbox = (CheckBoxHelper(htmlHelper, metadata, htmlFieldName, htmlHelper.ViewData.Model != null ? expression.Compile()(htmlHelper.ViewData.Model) : (bool?)null, AnonymousObjectToHtmlAttributes(checkboxAttributes)).ToHtmlString());
            var labelHtmlAttributes = AnonymousObjectToHtmlAttributes(labelAttributes);
            // add checkbox style to label, for Bootstrap
            if (labelHtmlAttributes.ContainsKey("class"))
                labelHtmlAttributes["class"] += " checkbox";
            else
                labelHtmlAttributes["class"] = "checkbox";

            text = text ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            return LabelHelper(htmlHelper, metadata,
                               htmlFieldName,
                               labelHtmlAttributes,
                               checkbox + text);
        }

        public static MvcHtmlString Label(this HtmlHelper htmlHelper, string labelFor, object htmlAttributes,
                                          string text = null)
        {
            return LabelHelper(htmlHelper, ModelMetadata.FromStringExpression(labelFor, htmlHelper.ViewData),
                               labelFor, new RouteValueDictionary(htmlAttributes), text);
        }


        public static string AbsoluteContent(this UrlHelper url, string path)
        {
            Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);

            //If the URI is not already absolute, rebuild it based on the current request.
            if (!uri.IsAbsoluteUri)
            {
                Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
                UriBuilder builder = new UriBuilder(requestUrl.Scheme, requestUrl.Host, requestUrl.Port)
                {
                    Path =
                        VirtualPathUtility.ToAbsolute("~/" + path)
                };

                uri = builder.Uri;
            }

            return uri.ToString();
        }

        public static MvcHtmlString Link(this HtmlHelper helper, string text, string url, object htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("href", ParseUrl(url));
            if (htmlAttributes != null)
            {
                var dictionary = AnonymousObjectToHtmlAttributes(htmlAttributes);
                dictionary.ForEach(pair => tagBuilder.Attributes.Add(pair.Key, Convert.ToString(pair.Value)));
            }
            tagBuilder.InnerHtml = text;

            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        private static string ParseUrl(string url)
        {
            Uri result;
            if (Uri.TryCreate(url, UriKind.Absolute, out result))
            {
                return result.ToString();
            }
            if (Uri.TryCreate("http://" + url, UriKind.Absolute, out result))
            {
                return result.ToString();
            }
            return url;
        }

        public static string AssemblyVersion(this HtmlHelper html)
        {
            var fileVersion =
                typeof(MrCMSApplication)
                    .Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)
                    .OfType<AssemblyFileVersionAttribute>()
                    .FirstOrDefault();
            return fileVersion != null ? fileVersion.Version : null;
        }

        public static HtmlHelper GetHtmlHelper(this Controller controller)
        {
            var viewContext = new ViewContext(controller.ControllerContext, new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
            return new HtmlHelper(viewContext, new ViewPage());
        }

        private class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }

        public static RouteValueDictionary Merge(this RouteValueDictionary baseDictionary, RouteValueDictionary additions)
        {
            foreach (var key in additions)
                baseDictionary[key.Key] = key.Value;

            return baseDictionary;
        }

        public static RouteValueDictionary AnonymousObjectToHtmlAttributes(object htmlAttributes)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            if (htmlAttributes != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(htmlAttributes))
                    routeValueDictionary.Add(propertyDescriptor.Name.Replace('_', '-'), propertyDescriptor.GetValue(htmlAttributes));
            }
            return routeValueDictionary;
        }

        public static MvcHtmlString InfoBlock(this HtmlHelper helper, string boldText, string text)
        {
            var tagBulder = new TagBuilder("div");
            tagBulder.AddCssClass("alert alert-info");

            if (!string.IsNullOrEmpty(boldText))
            {
                var strongText = new TagBuilder("strong");
                strongText.SetInnerText(boldText);

                tagBulder.InnerHtml += strongText + text;

                return MvcHtmlString.Create(tagBulder.ToString());
            }
            tagBulder.SetInnerText(text);
            return MvcHtmlString.Create(tagBulder.ToString());
        }
    }
}