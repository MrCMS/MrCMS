using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Services;

namespace MrCMS.Shortcodes.Forms
{
    public class DropDownListRenderer : IFormElementRenderer<DropDownList>
    {
        public TagBuilder AppendElement(FormProperty formProperty)
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

            foreach (var option in formProperty.Options)
            {
                tagBuilder.InnerHtml += GetOption(option);
            }

            return tagBuilder;
        }

        private TagBuilder GetOption(FormListOption option)
        {
            var tagBuilder = new TagBuilder("option");
            tagBuilder.Attributes["value"] = option.Value;
            tagBuilder.InnerHtml += option.Value;
            if (option.Selected)
                tagBuilder.Attributes["selected"] = "selected";
            return tagBuilder;
        }

        public bool IsSelfClosing { get { return false; } }
    }
}