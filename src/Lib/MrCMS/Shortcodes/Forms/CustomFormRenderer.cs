using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;
using MrCMS.Website.Filters;

namespace MrCMS.Shortcodes.Forms
{
    public class CustomFormRenderer : ICustomFormRenderer
    {
        private readonly IElementRendererManager _elementRendererManager;
        private readonly ILabelRenderer _labelRenderer;
        private readonly IValidationMessaageRenderer _validationMessaageRenderer;
        private readonly ISubmittedMessageRenderer _submittedMessageRenderer;
        private readonly IConfigurationProvider _configurationProvider;

        public CustomFormRenderer(IElementRendererManager elementRendererManager, ILabelRenderer labelRenderer,
                                  IValidationMessaageRenderer validationMessaageRenderer,
                                  ISubmittedMessageRenderer submittedMessageRenderer, IConfigurationProvider configurationProvider)
        {
            _elementRendererManager = elementRendererManager;
            _labelRenderer = labelRenderer;
            _validationMessaageRenderer = validationMessaageRenderer;
            _submittedMessageRenderer = submittedMessageRenderer;
            _configurationProvider = configurationProvider;
        }

        public async Task<IHtmlContent> GetForm(IHtmlHelper helper, Form form1, FormSubmittedStatus submittedStatus)
        {
            if (form1 == null)
                return HtmlString.Empty;

            var formProperties = form1.FormProperties;
            if (!formProperties.Any())
                return HtmlString.Empty;

            var form = GetForm(form1);

            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            var formDesign = form1.FormDesign;
            formDesign = Regex.Replace(formDesign, "{label:([^}]+)}", AddLabel(formProperties));
            formDesign = Regex.Replace(formDesign, "{input:([^}]+)}", AddElement(formProperties, submittedStatus, siteSettings));
            formDesign = Regex.Replace(formDesign, "{validation:([^}]+)}", AddValidation(formProperties));
            formDesign = Regex.Replace(formDesign, "{submitted-message}", AddSubmittedMessage(form1, submittedStatus));
            formDesign = Regex.Replace(formDesign, "{recaptcha}", helper.RenderRecaptcha().GetString());
            formDesign = Regex.Replace(formDesign, "{gdpr}", DefaultFormRenderer.GetGDPRCheckbox(siteSettings.FormRendererType, siteSettings.GDPRFairProcessingText).GetString());
            form.InnerHtml.AppendHtml(formDesign);

            if (siteSettings.HasHoneyPot)
                form.InnerHtml.AppendHtml(siteSettings.GetHoneypot());

            return form;
        }

        private MatchEvaluator AddValidation(IList<FormProperty> formProperties)
        {
            return match =>
            {

                var formProperty =
                    formProperties.FirstOrDefault(
                        property => property.Name.Equals(match.Groups[1].Value, StringComparison.OrdinalIgnoreCase));
                return formProperty == null
                    ? string.Empty
                    : _validationMessaageRenderer.AppendRequiredMessage(formProperty).GetString();
            };
        }

        private string AddSubmittedMessage(Form form, FormSubmittedStatus submittedStatus)
        {
            if (!submittedStatus.Submitted) return string.Empty;

            return _submittedMessageRenderer.AppendSubmittedMessage(form, submittedStatus).GetString();
        }

        private MatchEvaluator AddLabel(IList<FormProperty> formProperties)
        {
            return match =>
                       {
                           var formProperty =
                               formProperties.FirstOrDefault(
                                   property => property.Name.Equals(match.Groups[1].Value, StringComparison.OrdinalIgnoreCase));
                           return formProperty == null
                               ? string.Empty
                               : _labelRenderer.AppendLabel(formProperty).GetString();
                       };
        }
        private MatchEvaluator AddElement(IList<FormProperty> formProperties, FormSubmittedStatus submittedStatus, SiteSettings siteSettings)
        {
            return match =>
                       {

                           var formProperty =
                               formProperties.FirstOrDefault(
                                   property => property.Name.Equals(match.Groups[1].Value, StringComparison.OrdinalIgnoreCase));
                           if (formProperty == null)
                               return string.Empty;
                           var existingValue = submittedStatus.Data[formProperty.Name];

                           var renderer = _elementRendererManager.GetPropertyRenderer(formProperty);

                           var element = renderer.AppendElement(formProperty, existingValue, siteSettings.FormRendererType);
                           element.TagRenderMode = renderer.IsSelfClosing
                               ? TagRenderMode.SelfClosing
                               : TagRenderMode.Normal;
                           return element.GetString();
                       };
        }

        public TagBuilder GetForm(Form form)
        {
            var tagBuilder = new TagBuilder("form");
            tagBuilder.Attributes["method"] = "POST";
            tagBuilder.Attributes["enctype"] = "multipart/form-data";
            tagBuilder.Attributes["action"] = $"/save-form/{form.Id}";

            return tagBuilder;
        }
    }
}