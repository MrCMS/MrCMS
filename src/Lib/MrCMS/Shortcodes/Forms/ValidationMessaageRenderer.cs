using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public class ValidationMessaageRenderer : IValidationMessaageRenderer
    {
        public TagBuilder AppendRequiredMessage(FormProperty formProperty)
        {
            return GetValidationMessage(formProperty.Name);
        }

        public static TagBuilder GetValidationMessage(string name)
        {
            var tagBuilder = new TagBuilder("span");
            tagBuilder.AddCssClass("field-validation-valid");
            tagBuilder.Attributes["data-valmsg-for"] = name; //.GetHtmlId();
            tagBuilder.Attributes["data-valmsg-replace"] = "true";
            tagBuilder.Attributes["id"] = $"{name}-error";
            //data-valmsg-for="LastName" data-valmsg-replace="true"
            return tagBuilder;
        }
    }
}