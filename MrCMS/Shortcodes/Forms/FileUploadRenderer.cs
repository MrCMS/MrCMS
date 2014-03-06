using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;

namespace MrCMS.Shortcodes.Forms
{
    public class FileUploadRenderer : IFormElementRenderer<FileUpload>
    {
        public TagBuilder AppendElement(FileUpload formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.Attributes["type"] = "file";
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
            return tagBuilder;
        }

        public TagBuilder AppendElement(FormProperty formProperty, string existingValue, FormRenderingType formRenderingType)
        {
            return AppendElement(formProperty as FileUpload, existingValue, formRenderingType);
        }

        public bool IsSelfClosing { get { return true; } }
    }
}