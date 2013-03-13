using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public class CustomFormRenderer : ICustomFormRenderer
    {
        private readonly IElementRendererManager _elementRendererManager;
        private readonly ILabelRenderer _labelRenderer;
        private IValidationMessaageRenderer _validationMessaageRenderer;

        public CustomFormRenderer(IElementRendererManager elementRendererManager, ILabelRenderer labelRenderer, IValidationMessaageRenderer validationMessaageRenderer)
        {
            _elementRendererManager = elementRendererManager;
            _labelRenderer = labelRenderer;
            _validationMessaageRenderer = validationMessaageRenderer;
        }
        public string GetForm(Webpage webpage, FormSubmittedStatus submittedStatus)
        {
            if (webpage == null)
                return string.Empty;

            var formProperties = webpage.FormProperties;
            if (!formProperties.Any())
                return string.Empty;

            var form = GetForm(webpage);

            var formDesign = webpage.FormDesign;
            formDesign = Regex.Replace(formDesign, "{label:([^}]+)}", AddLabel(formProperties));
            formDesign = Regex.Replace(formDesign, "{input:([^}]+)}", AddElement(formProperties));
            formDesign = Regex.Replace(formDesign, "{validation:([^}]+)}", AddValidation(formProperties));
            formDesign = Regex.Replace(formDesign, "{submitted-message}", AddSubmittedMessage(webpage, submittedStatus));
            form.InnerHtml = formDesign;
            return form.ToString();
        }

        private MatchEvaluator AddValidation(IList<FormProperty> formProperties)
        {
            return match =>
            {

                var formProperty =
                    formProperties.FirstOrDefault(
                        property => property.Name.Equals(match.Groups[1].Value, StringComparison.OrdinalIgnoreCase));
                if (formProperty == null)
                    return string.Empty;

                return _validationMessaageRenderer.AppendRequiredMessage(formProperty).ToString();
            };
        }

        private string AddSubmittedMessage(Webpage webpage, FormSubmittedStatus submittedStatus)
        {
            if (!submittedStatus.Submitted) return string.Empty;

            var message = new TagBuilder("div");
            message.AddCssClass("alert");
            message.AddCssClass(submittedStatus.Success ? "alert-success" : "alert-error");
            message.InnerHtml +=
                "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">x</button>" +
                (submittedStatus.Error ?? (!string.IsNullOrWhiteSpace(webpage.FormSubmittedMessage)
                                               ? webpage.FormSubmittedMessage
                                               : "Form submitted"));

            return message.ToString();
        }

        private MatchEvaluator AddLabel(IList<FormProperty> formProperties)
        {
            return match =>
                       {

                           var formProperty =
                               formProperties.FirstOrDefault(
                                   property => property.Name.Equals(match.Groups[1].Value, StringComparison.OrdinalIgnoreCase));
                           if (formProperty == null)
                               return string.Empty;

                           return _labelRenderer.AppendLabel(formProperty).ToString();
                       };
        }
        private MatchEvaluator AddElement(IList<FormProperty> formProperties)
        {
            return match =>
                       {

                           var formProperty =
                               formProperties.FirstOrDefault(
                                   property => property.Name.Equals(match.Groups[1].Value, StringComparison.OrdinalIgnoreCase));
                           if (formProperty == null)
                               return string.Empty;

                           var renderer = _elementRendererManager.GetElementRenderer(formProperty);

                           return
                               renderer.AppendElement(formProperty)
                                       .ToString(renderer.IsSelfClosing
                                                     ? TagRenderMode.SelfClosing
                                                     : TagRenderMode.Normal);
                       };
        }

        public TagBuilder GetForm(Webpage webpage)
        {
            var tagBuilder = new TagBuilder("form");
            tagBuilder.Attributes["method"] = "POST";
            tagBuilder.Attributes["enctype"] = "multipart/form-data";
            tagBuilder.Attributes["action"] = string.Format("/save-form/{0}", webpage.Id);

            return tagBuilder;
        }
    }
}