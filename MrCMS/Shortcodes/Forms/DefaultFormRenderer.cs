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

        public DefaultFormRenderer(IElementRendererManager elementRendererManager, ILabelRenderer labelRenderer, IValidationMessaageRenderer validationMessaageRenderer)
        {
            _elementRendererManager = elementRendererManager;
            _labelRenderer = labelRenderer;
            _validationMessaageRenderer = validationMessaageRenderer;
        }

        public string GetDefault(Webpage webpage, FormSubmittedStatus submittedStatus)
        {
            if (webpage == null)
                return string.Empty;

            var formProperties = webpage.FormProperties;
            if (!formProperties.Any())
                return string.Empty;

            var form = GetForm(webpage);
            foreach (var property in formProperties)
            {
                IFormElementRenderer renderer = _elementRendererManager.GetElementRenderer(property);
                form.InnerHtml += _labelRenderer.AppendLabel(property);
                form.InnerHtml += renderer.AppendElement(property)
                                          .ToString(renderer.IsSelfClosing
                                                        ? TagRenderMode.SelfClosing
                                                        : TagRenderMode.Normal);
                form.InnerHtml += _validationMessaageRenderer.AppendRequiredMessage(property);
            }

            var div = new TagBuilder("div");
            div.InnerHtml += GetSubmitButton().ToString(TagRenderMode.SelfClosing);
            form.InnerHtml += div;
            
            if (submittedStatus.Submitted)
                form.InnerHtml += GetSubmittedMessage(webpage,submittedStatus);

            return form.ToString();
        }

        public TagBuilder GetSubmittedMessage(Webpage webpage, FormSubmittedStatus submittedStatus)
        {
            var message = new TagBuilder("div");
            message.AddCssClass("alert");
            message.AddCssClass(submittedStatus.Success ? "alert-success" : "alert-error");
            message.InnerHtml +=
                "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">x</button>" +
                (submittedStatus.Error ?? (!string.IsNullOrWhiteSpace(webpage.FormSubmittedMessage)
                                               ? webpage.FormSubmittedMessage
                                               : "Form submitted"));

            return message;
        }

        public TagBuilder GetSubmitButton()
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes["type"] = "submit";
            tagBuilder.Attributes["value"] = "Submit";
            tagBuilder.AddCssClass("btn");
            tagBuilder.AddCssClass("btn-primary");

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