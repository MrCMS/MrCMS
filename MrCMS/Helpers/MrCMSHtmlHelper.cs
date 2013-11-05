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
using MrCMS.Settings;
using MrCMS.Shortcodes;
using MrCMS.Website;
using MrCMS.Website.Optimization;
using Newtonsoft.Json;

namespace MrCMS.Helpers
{
    public static class HoneypotHtmlHelper
    {
        public static MvcHtmlString Honeypot(this HtmlHelper html)
        {
            var siteSettings = MrCMSApplication.Get<SiteSettings>();

            return siteSettings.HasHoneyPot
                       ? MvcHtmlString.Create(siteSettings.GetHoneypot().ToString())
                       : MvcHtmlString.Empty;
        }
    }
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

        public static IHtmlString ParseTweet(this string tweet)
        {
            var link =
                new Regex(@"http(s)?://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&amp;\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?");
            var screenName = new Regex(@"@\w+");
            var hashTag = new Regex(@"#\w+");

            string formattedTweet = link.Replace(tweet, match =>
            {
                string val = match.Value;
                return "<a href='" + val + "'>" + val + "</a>";
            });

            formattedTweet = screenName.Replace(formattedTweet, match =>
            {
                string val = match.Value.Trim('@');
                return
                    String.Format(
                        "@<a href='http://twitter.com/{0}'>{1}</a>",
                        val, val);
            });

            formattedTweet = hashTag.Replace(formattedTweet, match =>
            {
                string val = match.Value;
                return
                    String.Format(
                        "<a href='http://twitter.com/search/?q=%23{0}'>{1}</a>",
                        val.Substring(1), val);
            });

            return new HtmlString(formattedTweet);
        }

        public static IHtmlString ParseShortcodes(this HtmlHelper htmlHelper, string content)
        {
            var shortcodeParsers = MrCMSApplication.GetAll<IShortcodeParser>();

            content = shortcodeParsers.Aggregate(content, (current, shortcodeParser) => shortcodeParser.Parse(htmlHelper, current));

            return new HtmlString(content);
        }

        public static IHtmlString RenderBodyContent(this HtmlHelper htmlHelper, string content)
        {
            var htmlContent = htmlHelper.ParseShortcodes(content);

            if (!CurrentRequestData.CurrentUserIsAdmin)
                return htmlContent;

            var sb = new StringBuilder();
            sb.Append("<div class='editable'>" + htmlContent + "</div>");

            return new HtmlString(sb.ToString());
        }

        public static MvcHtmlString RenderCustomAdminProperties<T>(this HtmlHelper<T> htmlHelper)
        {
            T model = htmlHelper.ViewData.Model;
            if (model == null)
                return MvcHtmlString.Empty;
            if (MrCMSApp.AppWebpages.ContainsKey(model.GetType()))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppWebpages[model.GetType()];
            if (MrCMSApp.AppWidgets.ContainsKey(model.GetType()))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppWidgets[model.GetType()];

            ViewEngineResult viewEngineResult =
                ViewEngines.Engines.FindView(
                    new ControllerContext(htmlHelper.ViewContext.RequestContext, htmlHelper.ViewContext.Controller),
                    model.GetType().Name, "");
            if (viewEngineResult.View != null)
            {
                try
                {
                    return htmlHelper.Partial(model.GetType().Name, model);
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                    return
                        MvcHtmlString.Create("We could not find a custom admin view for this page. Either this page is bespoke or has no custom properties.");
                }
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString RenderUserOwnedObjectProperties(this HtmlHelper<User> htmlHelper, Type entityType)
        {
            var user = htmlHelper.ViewData.Model;
            if (user == null)
                return MvcHtmlString.Empty;
            if (MrCMSApp.AppEntities.ContainsKey(entityType))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppEntities[entityType];

            ViewEngineResult viewEngineResult =
                ViewEngines.Engines.FindView(new ControllerContext(htmlHelper.ViewContext.RequestContext, htmlHelper.ViewContext.Controller), entityType.Name, "");
            if (viewEngineResult.View != null)
            {
                try
                {
                    return htmlHelper.Partial(entityType.Name, user);
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                }
            }
            return MvcHtmlString.Empty;
        }
        public static MvcHtmlString RenderUserProfileProperties(this HtmlHelper<User> htmlHelper, Type userProfileType)
        {
            var user = htmlHelper.ViewData.Model;
            if (user == null)
                return MvcHtmlString.Empty;
            if (MrCMSApp.AppUserProfileDatas.ContainsKey(userProfileType))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppUserProfileDatas[userProfileType];

            ViewEngineResult viewEngineResult =
                ViewEngines.Engines.FindView(new ControllerContext(htmlHelper.ViewContext.RequestContext, htmlHelper.ViewContext.Controller), userProfileType.Name, "");
            if (viewEngineResult.View != null)
            {
                try
                {
                    return htmlHelper.Partial(userProfileType.Name, user);
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                }
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString RenderValue(this HtmlHelper htmlHelper, object value)
        {
            return value != null
                       ? MvcHtmlString.Create(IsJson(value.ToString())
                                                  ? GetPrettyPrintedJson(value.ToString())
                                                  : value.ToString())
                       : MvcHtmlString.Create(String.Empty);
        }

        private static string GetPrettyPrintedJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            var prettyPrintedJson = string.Format("<pre>{0}</pre>",
                                                  JsonConvert.SerializeObject(parsedJson, Formatting.Indented));
            return prettyPrintedJson;
        }

        private static bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, FormMethod formMethod, object htmlAttributes)
        {
            // generates <form action="{current url}" method="post">...</form> 
            string formAction = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            return FormHelper(htmlHelper, formAction, formMethod, AnonymousObjectToHtmlAttributes(htmlAttributes));
        }


        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification =
                "Because disposing the object would write to the response stream, you don't want to prematurely dispose of this object."
            )]
        private static MvcForm FormHelper(this HtmlHelper htmlHelper, string formAction, FormMethod method,
                                          IDictionary<string, object> htmlAttributes)
        {
            var tagBuilder = new TagBuilder("form");
            tagBuilder.MergeAttributes(htmlAttributes);
            // action is implicitly generated, so htmlAttributes take precedence.
            tagBuilder.MergeAttribute("action", formAction);
            // method is an explicit parameter, so it takes precedence over the htmlAttributes. 
            tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);

            htmlHelper.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            var theForm = new MvcForm(htmlHelper.ViewContext);

            return theForm;
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

        public static MvcHtmlString RenderImage(this HtmlHelper helper, string imageUrl, string alt = null, string title = null, object attributes = null)
        {
            if (String.IsNullOrWhiteSpace(imageUrl))
                return MvcHtmlString.Empty;

            var image = MrCMSApplication.Get<IImageProcessor>().GetImage(imageUrl);
            var tagBuilder = new TagBuilder("img");
            if (image == null)
                return MvcHtmlString.Empty;

            tagBuilder.Attributes.Add("src", imageUrl);
            tagBuilder.Attributes.Add("alt", alt ?? image.Description);
            tagBuilder.Attributes.Add("title", title ?? image.Title);
            if (attributes != null)
            {
                var routeValueDictionary = AnonymousObjectToHtmlAttributes(attributes);
                foreach (var kvp in routeValueDictionary)
                {
                    tagBuilder.Attributes.Add(kvp.Key, kvp.Value.ToString());
                }
            }
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString RenderImage(this HtmlHelper helper, string imageUrl, Size targetSize, string alt = null, string title = null, object attributes = null)
        {
            if (String.IsNullOrWhiteSpace(imageUrl))
                return MvcHtmlString.Empty;

            var imageProcessor = MrCMSApplication.Get<IImageProcessor>();
            var fileService = MrCMSApplication.Get<IFileService>();
            var image = imageProcessor.GetImage(imageUrl);

            if (image == null || targetSize == Size.Empty)
                return MvcHtmlString.Empty;

            if (ImageProcessor.RequiresResize(image.Size, targetSize))
            {
                var location = fileService.GetFileLocation(image, targetSize);
                if (!string.IsNullOrWhiteSpace(location))
                    imageUrl = location;
            }

            var tagBuilder = new TagBuilder("img");
            tagBuilder.Attributes.Add("src", imageUrl);
            tagBuilder.Attributes.Add("alt", alt ?? image.Title);
            tagBuilder.Attributes.Add("title", title ?? image.Description);
            if (attributes != null)
            {
                var routeValueDictionary = AnonymousObjectToHtmlAttributes(attributes);
                foreach (var kvp in routeValueDictionary)
                {
                    tagBuilder.Attributes.Add(kvp.Key, kvp.Value.ToString());
                }
            }
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
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

        public static HtmlHelper GetHtmlHelper(Controller controller)
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

        public static void IncludeScript(this HtmlHelper helper, string url)
        {
            var webPage = helper.ViewDataContainer as WebPageBase;
            var virtualPath = webPage == null ? string.Empty : webPage.VirtualPath;
            MrCMSApplication.Get<IResourceBundler>().AddScript(virtualPath, url);
        }

        public static MvcHtmlString RenderScripts(this HtmlHelper helper)
        {
            return MrCMSApplication.Get<IResourceBundler>().GetScripts();
        }

        public static void IncludeCss(this HtmlHelper helper, string url)
        {
            var webPage = helper.ViewDataContainer as WebPageBase;
            var virtualPath = webPage == null ? string.Empty : webPage.VirtualPath;
            MrCMSApplication.Get<IResourceBundler>().AddCss(virtualPath, url);
        }

        public static MvcHtmlString RenderCss(this HtmlHelper helper)
        {
            return MrCMSApplication.Get<IResourceBundler>().GetCss();
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

        public static List<string> SuccessMessages(this TempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("success-message"))
            {
                tempData["success-message"] = new List<string>();
            }
            return tempData["success-message"] as List<string>;
        }

        public static List<string> ErrorMessages(this TempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("error-message"))
            {
                tempData["error-message"] = new List<string>();
            }
            return tempData["error-message"] as List<string>;
        }

        public static List<string> InfoMessages(this TempDataDictionary tempData)
        {
            if (!tempData.ContainsKey("info-message"))
            {
                tempData["info-message"] = new List<string>();
            }
            return tempData["info-message"] as List<string>;
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