using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MrCMS.Helpers
{
    public static class MrCMSHtmlHelperExtensions
    {
        private static IHtmlContent CheckBoxHelper<TModel>(IHtmlHelper<TModel> htmlHelper, ModelMetadata metadata,
            string name, bool? isChecked,
            IDictionary<string, object> htmlAttributes)
        {
            var explicitValue = isChecked.HasValue;
            if (explicitValue)
            {
                htmlAttributes.Remove("checked"); // Explicit value must override dictionary
            }

            return MakeCheckbox(htmlHelper, InputType.CheckBox, metadata, typeof(TModel), htmlHelper.ViewData.Model,
                name, !explicitValue /* useViewData */,
                isChecked ?? false, htmlAttributes);
        }

        private static IHtmlContent MakeCheckbox(IHtmlHelper htmlHelper, InputType inputType, ModelMetadata metadata,
            Type type,
            object viewDataModel,
            string name, bool useViewData, bool isChecked, IDictionary<string, object> htmlAttributes)
        {
            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("type", GetInputTypeString(inputType));
            tagBuilder.MergeAttribute("type", GetInputTypeString(inputType));
            tagBuilder.MergeAttribute("name", fullName, true);
            tagBuilder.MergeAttribute("class", "form-check-input");

            var valueParameter = Convert.ToString("true", CultureInfo.CurrentCulture);
            var usedModelState = false;

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
                    isChecked = string.Equals(modelStateValue, valueParameter, StringComparison.Ordinal);
                    usedModelState = true;
                }
            }

            if (!usedModelState && useViewData)
            {
                isChecked = htmlHelper.EvalBoolean(fullName);
            }

            if (isChecked)
            {
                tagBuilder.MergeAttribute("checked", "checked");
            }

            tagBuilder.MergeAttribute("value", valueParameter, false);

            tagBuilder.GenerateId(fullName, "-");

            // If there are any errors for a named field, we add the css attribute.
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out ModelStateEntry modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            AddValidationAttributes(htmlHelper.ViewContext, tagBuilder,
                htmlHelper.MetadataProvider.GetModelExplorerForType(type, viewDataModel), name);
            //tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            if (inputType == InputType.CheckBox)
            {
                // Render an additional <input type="hidden".../> for checkboxes. This 
                // addresses scenarios where unchecked checkboxes are not sent in the request.
                // Sending a hidden input makes it possible to know that the checkbox was present 
                // on the page when the request was submitted. 
                IHtmlContentBuilder inputItemBuilder = new HtmlContentBuilder();
                tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
                inputItemBuilder = inputItemBuilder.AppendHtml(tagBuilder);

                var hiddenInput = new TagBuilder("input");
                hiddenInput.MergeAttribute("type", GetInputTypeString(InputType.Hidden));
                hiddenInput.MergeAttribute("name", fullName);
                hiddenInput.MergeAttribute("value", "false");
                hiddenInput.TagRenderMode = TagRenderMode.SelfClosing;
                inputItemBuilder.AppendHtml(hiddenInput);
                return inputItemBuilder;
            }

            return tagBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
        }


        /// <summary>
        ///     Adds validation attributes to the <paramref name="tagBuilder" /> if client validation
        ///     is enabled.
        /// </summary>
        /// <param name="viewContext">
        ///     A <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.ViewContext" /> instance for the current
        ///     scope.
        /// </param>
        /// <param name="tagBuilder">A <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.TagBuilder" /> instance.</param>
        /// <param name="modelExplorer">
        ///     The <see cref="T:Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExplorer" /> for the
        ///     <paramref name="expression" />.
        /// </param>
        /// <param name="expression">Expression name, relative to the current model.</param>
        private static void AddValidationAttributes(ViewContext viewContext, TagBuilder tagBuilder,
            ModelExplorer modelExplorer, string expression)
        {
            var provider = new DefaultValidationHtmlAttributeProvider(
                new OptionsWrapper<MvcViewOptions>(new MvcViewOptions()), modelExplorer.Metadata,
                new ClientValidatorCache());
            provider.AddAndTrackValidationAttributes(viewContext, modelExplorer, expression, tagBuilder.Attributes);
        }

        private static string GetInputTypeString(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.CheckBox:
                    return "checkbox";
                case InputType.Hidden:
                    return "hidden";
                case InputType.Password:
                    return "password";
                case InputType.Radio:
                    return "radio";
                case InputType.Text:
                    return "text";
                default:
                    return "text";
            }
        }


        internal static IHtmlContent LabelHelper(IHtmlHelper html, ModelMetadata metadata, string htmlFieldName,
            IDictionary<string, object> htmlAttributes, IHtmlContent labelText = null)
        {
            var resolvedLabelText = labelText ??
                                    new HtmlString(metadata.DisplayName ??
                                                   metadata.PropertyName ?? htmlFieldName.Split('.').Last());

            return GetLabelTag(html, htmlFieldName, htmlAttributes, resolvedLabelText);
        }

        private static IHtmlContent GetLabelTag(IHtmlHelper html, string htmlFieldName, IDictionary<string, object> htmlAttributes,
            IHtmlContent resolvedLabelText)
        {
            var tag = new TagBuilder("label");
            tag.Attributes.Add("for",
                TagBuilder.CreateSanitizedId(
                    html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName), "-"));

            if (htmlAttributes != null)
            {
                tag.MergeAttributes(htmlAttributes);
            }

            tag.InnerHtml.AppendHtml(resolvedLabelText);
            return tag.ToMvcHtmlString(TagRenderMode.Normal);
        }

        private static object GetModelStateValue(this IHtmlHelper html, string key, Type destinationType)
        {
            if (html.ViewData.ModelState.TryGetValue(key, out ModelStateEntry modelState) && modelState.RawValue != null)
            {
                return modelState.RawValue.To(destinationType, null /* culture */);
            }

            return null;
        }

        private static IHtmlContent ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
        {
            tagBuilder.TagRenderMode = renderMode;
            return tagBuilder;
        }

        private static string EvalString(this IHtmlHelper html, string key)
        {
            return Convert.ToString(html.ViewData.Eval(key), CultureInfo.CurrentCulture);
        }

        private static bool EvalBoolean(this IHtmlHelper html, string key)
        {
            return Convert.ToBoolean(html.ViewData.Eval(key), CultureInfo.InvariantCulture);
        }

        public static IHtmlContent FormLink(this IHtmlHelper htmlHelper, string linkText, string actionName,
            string controllerName, object routeValues, object htmlAttributes)
        {
            var routeValuesDictionary = new RouteValueDictionary(routeValues);
            var htmlAttributeDictionary = new RouteValueDictionary(htmlAttributes);

            htmlAttributeDictionary["data-action"] = "post-link";
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValuesDictionary,
                htmlAttributeDictionary);
        }

        public static IHtmlContent AjaxFormLink(this IHtmlHelper htmlHelper, string linkText, string actionName,
            string controllerName, object routeValues, object htmlAttributes)
        {
            var routeValuesDictionary = new RouteValueDictionary(routeValues);
            IDictionary<string, object> htmlAttributeDictionary = AnonymousObjectToHtmlAttributes(htmlAttributes);
            htmlAttributeDictionary.Add("data-action", "post-link-ajax");
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValuesDictionary,
                htmlAttributeDictionary);
        }

        public static IHtmlContent AjaxFormLink(this IHtmlHelper htmlHelper, string linkText, string actionName,
            object routeValues, object htmlAttributes)
        {
            return AjaxFormLink(htmlHelper, linkText, actionName, null, routeValues, htmlAttributes);
        }

        public static IHtmlContent FormLink(this IHtmlHelper htmlHelper, string linkText, string actionName,
            object routeValues, object htmlAttributes)
        {
            return FormLink(htmlHelper, linkText, actionName, null, routeValues, htmlAttributes);
        }


        public static IHtmlContent LabelFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, object>> expression, object htmlAttributes,
            string text = null)
        {
            var modelExpressionProvider = htmlHelper.GetRequiredService<ModelExpressionProvider>();
            return LabelHelper(htmlHelper,
                modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression).Metadata,
                modelExpressionProvider.GetExpressionText(expression), AnonymousObjectToHtmlAttributes(htmlAttributes),
                text != null ? new HtmlString(text) : null);
        }

        public static IHtmlContent InlineCheckboxFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression, object labelAttributes = null, object checkboxAttributes = null,
            string text = null)
        {
            var modelExpressionProvider = htmlHelper.GetRequiredService<ModelExpressionProvider>();
            var metadata = modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression).Metadata;
            var htmlFieldName = modelExpressionProvider.GetExpressionText(expression);
            var checkbox =
                CheckBoxHelper(htmlHelper, metadata, htmlFieldName,
                    htmlHelper.ViewData.Model != null ? expression.Compile()(htmlHelper.ViewData.Model) : (bool?)null,
                    AnonymousObjectToHtmlAttributes(checkboxAttributes));
            var labelHtmlAttributes = AnonymousObjectToHtmlAttributes(labelAttributes);
            // add checkbox style to label, for Bootstrap
            if (labelHtmlAttributes.ContainsKey("class"))
            {
                labelHtmlAttributes["class"] += " form-check-label";
            }
            else
            {
                labelHtmlAttributes["class"] = "form-check-label";
            }

            text = text ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            var builder = new HtmlContentBuilder().AppendHtml(checkbox).Append(text);
            return LabelHelper(htmlHelper, metadata,
                htmlFieldName,
                labelHtmlAttributes,
                builder);
        }

        public static IHtmlContent Label(this IHtmlHelper htmlHelper, string labelFor, object htmlAttributes,
            string text = null)
        {
            return GetLabelTag(htmlHelper, labelFor,
                htmlAttributes == null ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes),
                new HtmlString(text ?? labelFor.BreakUpString()));
        }

        public static IHtmlContent Link(this IHtmlHelper helper, string text, string url, object htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes.Add("href", ParseUrl(url, helper.ViewContext.HttpContext.Request.Scheme));
            if (htmlAttributes != null)
            {
                var dictionary = AnonymousObjectToHtmlAttributes(htmlAttributes);
                dictionary.ForEach(pair => tagBuilder.Attributes.Add(pair.Key, Convert.ToString(pair.Value)));
            }

            tagBuilder.InnerHtml.AppendHtml(text);

            return tagBuilder;
        }

        private static string ParseUrl(string url, string scheme)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                return result.ToString();
            }

            if (Uri.TryCreate(scheme + "://" + url, UriKind.Absolute, out result))
            {
                return result.ToString();
            }

            return url;
        }

        public static string AssemblyVersion(this IHtmlHelper html)
        {
            var fileVersion =
                typeof(MrCMSHtmlHelperExtensions)
                    .Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)
                    .OfType<AssemblyFileVersionAttribute>()
                    .FirstOrDefault();
            return fileVersion?.Version;
        }


        //public static IHtmlHelper GetWrappedHtml(this HtmlHelper helper)
        //{
        //    return new Website.MrCMSHtmlHelper(helper);
        //}

        public static IHtmlHelper GetHtmlHelper(this Controller controller)
        {
            var htmlHelper = (HtmlHelper)controller.HttpContext.RequestServices.GetRequiredService<IHtmlHelper>();
            htmlHelper.Contextualize(new ViewContext());
            return htmlHelper;
        }

        public static RouteValueDictionary Merge(this RouteValueDictionary baseDictionary,
            RouteValueDictionary additions)
        {
            foreach (var key in additions)
            {
                baseDictionary[key.Key] = key.Value;
            }

            return baseDictionary;
        }

        public static RouteValueDictionary AnonymousObjectToHtmlAttributes(object htmlAttributes)
        {
            var routeValueDictionary = new RouteValueDictionary();
            if (htmlAttributes != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(htmlAttributes))
                {
                    routeValueDictionary.Add(propertyDescriptor.Name.Replace('_', '-'),
                        propertyDescriptor.GetValue(htmlAttributes));
                }
            }

            return routeValueDictionary;
        }

        public static IHtmlContent InfoBlock(this IHtmlHelper helper, string boldText, string text,
            AlertType alertType = AlertType.info)
        {
            var tagBulder = new TagBuilder("div");
            tagBulder.AddCssClass("alert alert-" + alertType);

            if (!string.IsNullOrEmpty(boldText))
            {
                var strongText = new TagBuilder("strong");
                strongText.InnerHtml.Append(boldText);

                tagBulder.InnerHtml.AppendHtml(strongText);
                tagBulder.InnerHtml.AppendHtml(" " + text);
            }
            else
            {
                tagBulder.InnerHtml.AppendHtml(text);
            }

            return tagBulder;
        }

        private class FakeView : IView
        {
            public Task RenderAsync(ViewContext context)
            {
                throw new InvalidOperationException();
            }

            public string Path { get; set; }
        }
    }
}
