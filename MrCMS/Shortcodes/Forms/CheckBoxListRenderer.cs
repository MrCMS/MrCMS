using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using System.Linq;

namespace MrCMS.Shortcodes.Forms
{
    public class CheckBoxListRenderer : IFormElementRenderer<CheckboxList>
    {
        public const string CbHiddenValue = "cb-hidden-value";

        public TagBuilder AppendElement(FormProperty formProperty, string existingValue)
        {
            var values = existingValue == null
                             ? new List<string>()
                             : existingValue.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(s => s.Trim())
                                            .ToList();
            values.Remove(CbHiddenValue);

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

                if (existingValue != null)
                {
                    if (values.Contains(checkbox.Value))
                        checkboxBuilder.Attributes["checked"] = "checked";
                }
                else if (checkbox.Selected)
                    checkboxBuilder.Attributes["checked"] = "checked";

                checkboxBuilder.Attributes["name"] = formProperty.Name;
                checkboxBuilder.Attributes["id"] = TagBuilder.CreateSanitizedId(formProperty.Name + "-" + checkbox.Value);
                cbLabelBuilder.InnerHtml += checkboxBuilder.ToString();
                tagBuilder.InnerHtml += cbLabelBuilder.ToString();
            }

            var cbHiddenBuilder = new TagBuilder("input");
            cbHiddenBuilder.Attributes["type"] = "hidden";
            cbHiddenBuilder.Attributes["name"] = formProperty.Name;
            cbHiddenBuilder.Attributes["value"] = CbHiddenValue;
            tagBuilder.InnerHtml += cbHiddenBuilder.ToString();

            return tagBuilder;
        }

        public bool IsSelfClosing { get { return false; } }
    }
}