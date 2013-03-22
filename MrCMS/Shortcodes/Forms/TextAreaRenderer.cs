using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public class TextAreaRenderer : IFormElementRenderer<TextArea>
    {
        public TagBuilder AppendElement(FormProperty formProperty, string existingValue)
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
            tagBuilder.InnerHtml = existingValue;
            return tagBuilder;
        }

        public bool IsSelfClosing { get { return false; } }
    }
}