using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public class DropDownListRenderer : IFormElementRenderer<DropDownList>
    {
        private readonly IDropDownListOptionRenderer _dropDownListOptionRenderer;

        public DropDownListRenderer(IDropDownListOptionRenderer dropDownListOptionRenderer)
        {
            _dropDownListOptionRenderer = dropDownListOptionRenderer;
        }

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
                tagBuilder.InnerHtml += _dropDownListOptionRenderer.GetOption(option, existingValue);
            }
            return tagBuilder;
        }

        public bool IsSelfClosing { get { return false; } }
    }
}