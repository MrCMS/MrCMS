using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public class LabelRenderer : ILabelRenderer
    {
        public TagBuilder AppendLabel(FormProperty formProperty)
        {
            var tagBuilder = new TagBuilder("label");

            tagBuilder.Attributes["for"] = formProperty.GetHtmlId();

            tagBuilder.InnerHtml = string.IsNullOrWhiteSpace(formProperty.LabelText)
                                       ? formProperty.Name
                                       : formProperty.LabelText;

            return tagBuilder;
        }
    }
}