using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public class DropDownListOptionRenderer : IDropDownListOptionRenderer
    {
        public TagBuilder GetOption(FormListOption option, string existingValue)
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
    }
}