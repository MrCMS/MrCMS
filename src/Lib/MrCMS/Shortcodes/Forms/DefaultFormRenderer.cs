using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;
using MrCMS.Website.Filters;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Services;

namespace MrCMS.Shortcodes.Forms
{
    public class DefaultFormRenderer : IDefaultFormRenderer
    {
        private readonly IElementRendererManager _elementRendererManager;
        private readonly IRepository<FormProperty> _formPropertyRepository;
        private readonly ILabelRenderer _labelRenderer;
        private readonly SiteSettings _siteSettings;
        private readonly ISubmittedMessageRenderer _submittedMessageRenderer;
        private readonly IValidationMessaageRenderer _validationMessageRenderer;

        public DefaultFormRenderer(IElementRendererManager elementRendererManager, 
            IRepository<FormProperty> formPropertyRepository,
            ILabelRenderer labelRenderer,
            IValidationMessaageRenderer validationMessageRenderer, ISubmittedMessageRenderer submittedMessageRenderer,
            SiteSettings siteSettings)
        {
            _elementRendererManager = elementRendererManager;
            _formPropertyRepository = formPropertyRepository;
            _labelRenderer = labelRenderer;
            _validationMessageRenderer = validationMessageRenderer;
            _submittedMessageRenderer = submittedMessageRenderer;
            _siteSettings = siteSettings;
        }

        public IHtmlContent GetDefault(IHtmlHelper helper, Form formEntity, FormSubmittedStatus submittedStatus)
        {
            if (formEntity == null)
            {
                return HtmlString.Empty;
            }

            var formProperties = _formPropertyRepository.Query().Where(x=>x.FormId == formEntity.Id).OrderBy(x => x.DisplayOrder).ToList();
            if (!formProperties.Any())
            {
                return HtmlString.Empty;
            }

            var form = GetForm(formEntity);
            var renderingType = _siteSettings.FormRendererType;
            foreach (var property in formProperties)
            {
                IHtmlContentBuilder elementHtml = new HtmlContentBuilder();
                var renderer = _elementRendererManager.GetPropertyRenderer(property);
                elementHtml.AppendHtml(_labelRenderer.AppendLabel(property));
                var existingValue = submittedStatus.Data[property.Name];

                var element = renderer.AppendElement(property, existingValue, renderingType);
                element.TagRenderMode = renderer.IsSelfClosing ? TagRenderMode.SelfClosing : TagRenderMode.Normal;
                elementHtml.AppendHtml(element);
                elementHtml.AppendHtml(_validationMessageRenderer.AppendRequiredMessage(property));
                var elementContainer =
                    _elementRendererManager.GetPropertyContainer(renderingType, property);
                if (elementContainer != null)
                {
                    elementContainer.InnerHtml.AppendHtml(elementHtml);
                    form.InnerHtml.AppendHtml(elementContainer);
                }
                else
                {
                    form.InnerHtml.AppendHtml(elementHtml);
                }
            }

            if (formEntity.ShowGDPRConsentBox)
            {
                form.InnerHtml.AppendHtml(GetGDPRCheckbox(renderingType, _siteSettings.GDPRFairProcessingText));
            }

            form.InnerHtml.AppendHtml(helper.RenderRecaptcha());

            var div = new TagBuilder("div");
            div.InnerHtml.AppendHtml(GetSubmitButton(formEntity));
            form.InnerHtml.AppendHtml(div);

            if (submittedStatus.Submitted)
            {
                form.InnerHtml.AppendHtml(new TagBuilder("br"));
                form.InnerHtml.AppendHtml(_submittedMessageRenderer.AppendSubmittedMessage(formEntity, submittedStatus));
            }

            if (_siteSettings.HasHoneyPot)
            {
                form.InnerHtml.AppendHtml(_siteSettings.GetHoneypot());
            }

            return form;
        }

        public static IHtmlContent GetGDPRCheckbox(FormRenderingType renderingType, string labelText)
        {
            var cbLabelBuilder = new TagBuilder("label");
            var sanitizedId = TagBuilder.CreateSanitizedId(FormPostingHandler.GDPRConsent, "-");
            cbLabelBuilder.Attributes["for"] = sanitizedId;

            var checkboxBuilder = new TagBuilder("input");
            checkboxBuilder.Attributes["type"] = "checkbox";
            checkboxBuilder.Attributes["value"] = "true";

            var requiredMessage = "GDPR Consent is required";
            checkboxBuilder.Attributes["data-val"] = "true";
            checkboxBuilder.Attributes["data-val-mandatory"] = null;
            checkboxBuilder.Attributes["data-val-required"] = requiredMessage;

            checkboxBuilder.Attributes["name"] = FormPostingHandler.GDPRConsent;
            checkboxBuilder.Attributes["id"] = sanitizedId;
            checkboxBuilder.AddCssClass("form-check-input");

            cbLabelBuilder.InnerHtml.AppendHtml(labelText);
            var checkboxContainer = new TagBuilder("div");

            if (renderingType == FormRenderingType.Bootstrap3)
            {
                cbLabelBuilder.InnerHtml.AppendHtml(checkboxBuilder);

                checkboxContainer.AddCssClass("checkbox");
                checkboxContainer.InnerHtml.AppendHtml(cbLabelBuilder);
                var wrapper = new TagBuilder("div");
                wrapper.AddCssClass("form-group");
                wrapper.InnerHtml.AppendHtml(checkboxContainer);
                wrapper.InnerHtml.AppendHtml(
                    ValidationMessaageRenderer.GetValidationMessage(FormPostingHandler.GDPRConsent));
                return wrapper;
            }

            if (renderingType == FormRenderingType.Bootstrap4)
            {
                cbLabelBuilder.AddCssClass("form-check-label");
                checkboxContainer.AddCssClass("form-check");
                checkboxContainer.InnerHtml.AppendHtml(checkboxBuilder);
                checkboxContainer.InnerHtml.AppendHtml(cbLabelBuilder);
                var wrapper = new TagBuilder("div");
                wrapper.AddCssClass("form-group");
                wrapper.InnerHtml.AppendHtml(checkboxContainer);
                wrapper.InnerHtml.AppendHtml(
                    ValidationMessaageRenderer.GetValidationMessage(FormPostingHandler.GDPRConsent));
                return wrapper;
            }

            cbLabelBuilder.InnerHtml.AppendHtml(checkboxBuilder);
            checkboxContainer.InnerHtml.AppendHtml(cbLabelBuilder);
            checkboxContainer.InnerHtml.AppendHtml(
                ValidationMessaageRenderer.GetValidationMessage(FormPostingHandler.GDPRConsent));
            return checkboxContainer;

        }

        public TagBuilder GetSubmitButton(Form form)
        {
            var tagBuilder = new TagBuilder("input") { TagRenderMode = TagRenderMode.SelfClosing };
            tagBuilder.Attributes["type"] = "submit";
            tagBuilder.Attributes["value"] = !string.IsNullOrWhiteSpace(form.SubmitButtonText)
                ? form.SubmitButtonText
                : "Submit";
            tagBuilder.AddCssClass(!string.IsNullOrWhiteSpace(form.SubmitButtonCssClass)
                ? form.SubmitButtonCssClass
                : "btn btn-primary");
            return tagBuilder;
        }

        public TagBuilder GetForm(Form form)
        {
            var tagBuilder = new TagBuilder("form");
            tagBuilder.Attributes["method"] = "POST";
            tagBuilder.Attributes["enctype"] = "multipart/form-data";
            tagBuilder.Attributes["action"] = string.Format("/save-form/{0}", form.Id);

            return tagBuilder;
        }
    }
}