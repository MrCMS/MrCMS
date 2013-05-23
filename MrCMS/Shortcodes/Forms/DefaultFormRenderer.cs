using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes.Forms
{
    public class DefaultFormRenderer : IDefaultFormRenderer
    {
        private readonly IElementRendererManager _elementRendererManager;
        private readonly ILabelRenderer _labelRenderer;
        private readonly IValidationMessaageRenderer _validationMessaageRenderer;
        private readonly ISubmittedMessageRenderer _submittedMessageRenderer;

        public DefaultFormRenderer(IElementRendererManager elementRendererManager, ILabelRenderer labelRenderer, IValidationMessaageRenderer validationMessaageRenderer, ISubmittedMessageRenderer submittedMessageRenderer)
        {
            _elementRendererManager = elementRendererManager;
            _labelRenderer = labelRenderer;
            _validationMessaageRenderer = validationMessaageRenderer;
            _submittedMessageRenderer = submittedMessageRenderer;
        }

        public string GetDefault(Webpage webpage, FormSubmittedStatus submittedStatus)
        {
            if (webpage == null)
                return string.Empty;

            var formProperties = webpage.FormProperties.OrderBy(x=>x.DisplayOrder);
            if (!formProperties.Any())
                return string.Empty;

            var form = GetForm(webpage);
            foreach (var property in formProperties)
            {
                IFormElementRenderer renderer = _elementRendererManager.GetElementRenderer(property);
                form.InnerHtml += _labelRenderer.AppendLabel(property);
                var existingValue = submittedStatus.Data[property.Name];
                form.InnerHtml += renderer.AppendElement(property, existingValue)
                                          .ToString(renderer.IsSelfClosing
                                                        ? TagRenderMode.SelfClosing
                                                        : TagRenderMode.Normal);
                form.InnerHtml += _validationMessaageRenderer.AppendRequiredMessage(property);
            }

            var div = new TagBuilder("div");
            div.InnerHtml += GetSubmitButton(webpage).ToString(TagRenderMode.SelfClosing);
            form.InnerHtml += div;

            if (submittedStatus.Submitted)
                form.InnerHtml += _submittedMessageRenderer.AppendSubmittedMessage(webpage, submittedStatus);

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