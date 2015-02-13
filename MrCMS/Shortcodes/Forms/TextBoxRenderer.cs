using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;

namespace MrCMS.Shortcodes.Forms
{
    public class TextBoxRenderer : IFormElementRenderer<TextBox>
    {
        public TagBuilder AppendElement(TextBox formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes["type"] = "text";
            tagBuilder.Attributes["name"] = formProperty.Name;
            tagBuilder.Attributes["id"] = formProperty.GetHtmlId();
            tagBuilder.Attributes["placeholder"] = formProperty.PlaceHolder;


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

            tagBuilder.Attributes["value"] = existingValue;
            return tagBuilder;
        }

        public TagBuilder AppendElement(FormProperty formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            return AppendElement(formProperty as TextBox, existingValue, formRenderingType);
        }

        public bool IsSelfClosing { get { return true; } }
    }
}