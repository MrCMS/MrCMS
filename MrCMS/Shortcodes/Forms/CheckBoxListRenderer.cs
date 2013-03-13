using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public class CheckBoxListRenderer : IFormElementRenderer<CheckboxList>
    {
        public TagBuilder AppendElement(FormProperty formProperty)
        {
            var tagBuilder = new TagBuilder("div");
            foreach (var checkbox in formProperty.Options)
            {
                var cbLabelBuilder = new TagBuilder("label");
                cbLabelBuilder.Attributes["for"] = TagBuilder.CreateSanitizedId(formProperty.Name + "-" + checkbox.Value);
                cbLabelBuilder.InnerHtml = checkbox.Value;
                cbLabelBuilder.AddCssClass("checkbox");
                
                var checkboxBuilder = new TagBuilder("input");
                checkboxBuilder.Attributes["type"] = "checkbox";
                checkboxBuilder.Attributes["value"] = checkbox.Value;
                checkboxBuilder.AddCssClass(formProperty.CssClass);

                if (checkbox.Selected)
                    checkboxBuilder.Attributes["checked"] = "checked";

                checkboxBuilder.Attributes["name"] = formProperty.Name;
                checkboxBuilder.Attributes["id"] = TagBuilder.CreateSanitizedId(formProperty.Name + "-" + checkbox.Value);
                cbLabelBuilder.InnerHtml += checkboxBuilder.ToString();
                tagBuilder.InnerHtml += cbLabelBuilder.ToString();
            }
            return tagBuilder;
        }

        public string GetId(FormProperty formProperty)
        {
            var s = string.IsNullOrWhiteSpace(formProperty.HtmlId) ? formProperty.Name : formProperty.HtmlId;
            return TagBuilder.CreateSanitizedId(s);
        }

        public bool IsSelfClosing { get { return false; } }
    }
}