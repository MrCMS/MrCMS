using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;

namespace MrCMS.Shortcodes.Forms
{
    public class TextAreaRenderer : IFormElementRenderer<TextArea>
    {
        public TagBuilder AppendElement(TextArea formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            var tagBuilder = new TagBuilder("textarea");
            tagBuilder.Attributes["name"] = formProperty.Name;
            tagBuilder.Attributes["id"] = formProperty.GetHtmlId();

            if (formProperty.Required)
            {
                tagBuilder.Attributes["data-val"] = "true";
                tagBuilder.Attributes["data-val-required"] =
                    string.Format("The field {0} is required",
                                  string.IsNullOrWhiteSpace(formProperty.LabelText)
                                      ? formProperty.Name
                                      : formProperty.LabelText);
            }
            if (!string.IsNullOrWhiteSpace(formProperty.CssClass))
                tagBuilder.AddCssClass(formProperty.CssClass);
            if (formRenderingType == FormRenderingType.Bootstrap3)
                tagBuilder.AddCssClass("form-control");
            tagBuilder.InnerHtml = existingValue;
            return tagBuilder;
        }

        public TagBuilder AppendElement(FormProperty formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            return AppendElement(formProperty as TextArea, existingValue, formRenderingType);
        }

        public bool IsSelfClosing { get { return false; } }
    }
}