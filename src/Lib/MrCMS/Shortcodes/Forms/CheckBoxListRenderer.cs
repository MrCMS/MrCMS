using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents.Web.FormProperties;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Settings;

namespace MrCMS.Shortcodes.Forms
{
    public class CheckBoxListRenderer : IFormElementRenderer<CheckboxList>
    {
        public const string CbHiddenValue = "cb-hidden-value";

        public TagBuilder AppendElement(CheckboxList formProperty, string existingValue,
            FormRenderingType formRenderingType)
        {
            var values = existingValue == null
                ? new List<string>()
                : existingValue.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
            values.Remove(CbHiddenValue);

            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("form-group");
            foreach (var checkbox in formProperty.Options)
            {
                var cbLabelBuilder = new TagBuilder("label");
                cbLabelBuilder.Attributes["for"] =
                    TagBuilder.CreateSanitizedId(formProperty.Name + "-" + checkbox.Value, "-");

                var checkboxBuilder = GetCheckbox(formProperty, existingValue, checkbox, values);
                checkboxBuilder.AddCssClass("form-check-input");


                cbLabelBuilder.InnerHtml.AppendHtml(checkbox.Value);
                var checkboxContainer = new TagBuilder("div");
                cbLabelBuilder.AddCssClass("form-check-label");
                checkboxContainer.AddCssClass("form-check");
                checkboxContainer.InnerHtml.AppendHtml(checkboxBuilder);
                checkboxContainer.InnerHtml.AppendHtml(cbLabelBuilder);
                tagBuilder.InnerHtml.AppendHtml(checkboxContainer);
            }
            

            var cbHiddenBuilder = new TagBuilder("input");
            cbHiddenBuilder.Attributes["type"] = "hidden";
            cbHiddenBuilder.Attributes["name"] = formProperty.Name;
            cbHiddenBuilder.Attributes["value"] = CbHiddenValue;
            tagBuilder.InnerHtml.AppendHtml(cbHiddenBuilder);

            return tagBuilder;
        }

        public static TagBuilder GetCheckbox(CheckboxList formProperty, string existingValue, FormListOption checkbox,
            List<string> values)
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
            checkboxBuilder.Attributes["id"] =
                TagBuilder.CreateSanitizedId(formProperty.Name + "-" + checkbox.Value, "-");
            return checkboxBuilder;
        }

        public TagBuilder AppendElement(FormProperty formProperty, string existingValue,
            FormRenderingType formRenderingType)
        {
            return AppendElement(formProperty as CheckboxList, existingValue, formRenderingType);
        }

        public bool IsSelfClosing => false;

        public bool SupportsFloatingLabel => false;
    }
}
