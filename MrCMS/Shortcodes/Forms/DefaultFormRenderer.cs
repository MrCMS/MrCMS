using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;

namespace MrCMS.Shortcodes.Forms
{
    public class DefaultFormRenderer : IDefaultFormRenderer
    {
        private readonly IElementRendererManager _elementRendererManager;
        private readonly ILabelRenderer _labelRenderer;
        private readonly IValidationMessaageRenderer _validationMessaageRenderer;
        private readonly ISubmittedMessageRenderer _submittedMessageRenderer;
        private readonly SiteSettings _siteSettings;

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

        public string GetDefault(Webpage webpage, FormSubmittedStatus submittedStatus)
        {
            if (webpage == null)
                return string.Empty;

            var formProperties = webpage.FormProperties.OrderBy(x => x.DisplayOrder);
            if (!formProperties.Any())
                return string.Empty;

            var form = GetForm(webpage);
            foreach (var property in formProperties)
            {
                string elementHtml = string.Empty;
                IFormElementRenderer renderer = _elementRendererManager.GetElementRenderer(property);
                elementHtml+= _labelRenderer.AppendLabel(property);
                var existingValue = submittedStatus.Data[property.Name];
                elementHtml += renderer.AppendElement(property, existingValue, _siteSettings.FormRendererType)
                                          .ToString(renderer.IsSelfClosing
                                                        ? TagRenderMode.SelfClosing
                                                        : TagRenderMode.Normal);
                elementHtml += _validationMessaageRenderer.AppendRequiredMessage(property);
                var elementContainer = _elementRendererManager.GetElementContainer(_siteSettings.FormRendererType, property);
                if (elementContainer != null)
                {
                    elementContainer.InnerHtml += elementHtml;
                    form.InnerHtml += elementContainer;
                }
                else
                    form.InnerHtml += elementHtml;
            }

            var div = new TagBuilder("div");
            div.InnerHtml += GetSubmitButton(webpage).ToString(TagRenderMode.SelfClosing);
            form.InnerHtml += div;

            if (submittedStatus.Submitted)
            {
                form.InnerHtml += new TagBuilder("br");
                form.InnerHtml += _submittedMessageRenderer.AppendSubmittedMessage(webpage, submittedStatus);
            }

            if (_siteSettings.HasHoneyPot)
                form.InnerHtml += _siteSettings.GetHoneypot();

            return form.ToString();
        }


        public TagBuilder GetSubmitButton(Webpage webpage)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes["type"] = "submit";
            tagBuilder.Attributes["value"] = !string.IsNullOrWhiteSpace(webpage.SubmitButtonText) ? webpage.SubmitButtonText : "Submit";
            tagBuilder.AddCssClass(!string.IsNullOrWhiteSpace(webpage.SubmitButtonCssClass) ? webpage.SubmitButtonCssClass : "btn btn-primary");
            return tagBuilder;
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