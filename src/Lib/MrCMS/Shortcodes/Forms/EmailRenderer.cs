using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;

namespace MrCMS.Shortcodes.Forms
{
    public class EmailRenderer : IFormElementRenderer<Email>
    {
        public TagBuilder AppendElement(Email formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes["type"] = "email";
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
            if (formRenderingType == FormRenderingType.Bootstrap3 || formRenderingType == FormRenderingType.Bootstrap4)
                tagBuilder.AddCssClass("form-control");

            tagBuilder.Attributes["value"] = existingValue;
            return tagBuilder;
        }

        public TagBuilder AppendElement(FormProperty formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            return AppendElement(formProperty as Email, existingValue, formRenderingType);
        }

        public bool IsSelfClosing => true;
    }
}