using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Services;

namespace MrCMS.Shortcodes.Forms
{
    public class DropDownListRenderer : IFormElementRenderer<DropDownList>
    {
        public TagBuilder AppendElement(FormProperty formProperty, string existingValue)
        {
            var tagBuilder = new TagBuilder("select");
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

            foreach (var option in formProperty.Options)
            {
                tagBuilder.InnerHtml += GetOption(option, existingValue);
            }
            return tagBuilder;
        }

        private TagBuilder GetOption(FormListOption option, string existingValue)
        {
            var tagBuilder = new TagBuilder("option");
            tagBuilder.Attributes["value"] = option.Value;
            tagBuilder.InnerHtml += option.Value;
            if (!string.IsNullOrWhiteSpace(existingValue))
            {
                if (option.Value == existingValue)
                    tagBuilder.Attributes["selected"] = "selected";
            }
            else if (option.Selected)
                tagBuilder.Attributes["selected"] = "selected";
            return tagBuilder;
        }

        public bool IsSelfClosing { get { return false; } }
    }
}