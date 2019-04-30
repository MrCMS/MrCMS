using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;
using MrCMS.Website.Filters;
using System.Linq;

namespace MrCMS.Shortcodes.Forms
{
    public class DefaultFormRenderer : IDefaultFormRenderer
    {
        private readonly IElementRendererManager _elementRendererManager;
        private readonly ILabelRenderer _labelRenderer;
        private readonly SiteSettings _siteSettings;
        private readonly ISubmittedMessageRenderer _submittedMessageRenderer;
        private readonly IValidationMessaageRenderer _validationMessaageRenderer;

        public DefaultFormRenderer(IElementRendererManager elementRendererManager, ILabelRenderer labelRenderer,
            IValidationMessaageRenderer validationMessaageRenderer, ISubmittedMessageRenderer submittedMessageRenderer,
            SiteSettings siteSettings)
        {
            _elementRendererManager = elementRendererManager;
            _labelRenderer = labelRenderer;
            _validationMessaageRenderer = validationMessaageRenderer;
            _submittedMessageRenderer = submittedMessageRenderer;
            _siteSettings = siteSettings;
        }

        public IHtmlContent GetDefault(IHtmlHelper helper, Form form1, FormSubmittedStatus submittedStatus)
        {
            if (form1 == null)
            {
                return HtmlString.Empty;
            }

            var formProperties = form1.FormProperties.OrderBy(x => x.DisplayOrder);
            if (!formProperties.Any())
            {
                return HtmlString.Empty;
            }

            var form = GetForm(form1);
            foreach (var property in formProperties)
            {
                IHtmlContentBuilder elementHtml = new HtmlContentBuilder();
                var renderer = _elementRendererManager.GetElementRenderer(property);
                elementHtml.AppendHtml(_labelRenderer.AppendLabel(property));
                var existingValue = submittedStatus.Data[property.Name];

                var element = renderer.AppendElement(property, existingValue, _siteSettings.FormRendererType);
                element.TagRenderMode = renderer.IsSelfClosing ? TagRenderMode.SelfClosing : TagRenderMode.Normal;
                elementHtml.AppendHtml(element);
                elementHtml.AppendHtml(_validationMessaageRenderer.AppendRequiredMessage(property));
                var elementContainer =
                    _elementRendererManager.GetElementContainer(_siteSettings.FormRendererType, property);
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

            form.InnerHtml.AppendHtml(helper.RenderRecaptcha().GetString());

            var div = new TagBuilder("div");
            div.InnerHtml.AppendHtml(GetSubmitButton(form1));
            form.InnerHtml.AppendHtml(div);

            if (submittedStatus.Submitted)
            {
                form.InnerHtml.AppendHtml(new TagBuilder("br"));
                form.InnerHtml.AppendHtml(_submittedMessageRenderer.AppendSubmittedMessage(form1, submittedStatus));
            }

            if (_siteSettings.HasHoneyPot)
            {
                form.InnerHtml.AppendHtml(_siteSettings.GetHoneypot());
            }

            return form;
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