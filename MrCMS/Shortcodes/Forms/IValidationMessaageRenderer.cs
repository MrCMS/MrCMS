using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public interface IValidationMessaageRenderer
    {
        TagBuilder AppendRequiredMessage(FormProperty formProperty);
    }
}