using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;
using System.Linq;
using MrCMS.Settings;

namespace MrCMS.Shortcodes.Forms
{
    public class CheckBoxListRenderer : IFormElementRenderer<CheckboxList>
    {
        public const string CbHiddenValue = "cb-hidden-value";

        public TagBuilder AppendElement(CheckboxList formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            var values = existingValue == null
                                ? new List<string>()
                                : existingValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
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

                var checkboxBuilder = GetCheckbox(formProperty, existingValue, checkbox, values);

                cbLabelBuilder.InnerHtml += checkboxBuilder.ToString();
                if (formRenderingType == FormRenderingType.Bootstrap3)
                {
                    var checkboxContainer = new TagBuilder("div");
                    checkboxContainer.AddCssClass("checkbox");
                    checkboxContainer.InnerHtml += cbLabelBuilder.ToString();
                    tagBuilder.InnerHtml += checkboxContainer;
                }
                else
                {
                    tagBuilder.InnerHtml += cbLabelBuilder;
                }
            }

            var cbHiddenBuilder = new TagBuilder("input");
            cbHiddenBuilder.Attributes["type"] = "hidden";
            cbHiddenBuilder.Attributes["name"] = formProperty.Name;
            cbHiddenBuilder.Attributes["value"] = CbHiddenValue;
            tagBuilder.InnerHtml += cbHiddenBuilder.ToString();

            return tagBuilder;
        }

        private static TagBuilder GetCheckbox(CheckboxList formProperty, string existingValue, FormListOption checkbox, List<string> values)
        {
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

            if (formProperty.Required)
            {
                var requiredMessage = string.Format("The field {0} is required",
                    string.IsNullOrWhiteSpace(formProperty.LabelText)
                        ? formProperty.Name
                        : formProperty.LabelText);
                checkboxBuilder.Attributes["data-val"] = "true";
                checkboxBuilder.Attributes["data-val-mandatory"] = requiredMessage;
                checkboxBuilder.Attributes["data-val-required"] = requiredMessage;
            }

            checkboxBuilder.Attributes["name"] = formProperty.Name;
            checkboxBuilder.Attributes["id"] = TagBuilder.CreateSanitizedId(formProperty.Name + "-" + checkbox.Value);
            return checkboxBuilder;
        }

        public TagBuilder AppendElement(FormProperty formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            return AppendElement(formProperty as CheckboxList, existingValue, formRenderingType);
        }

        public bool IsSelfClosing { get { return false; } }
    }
}