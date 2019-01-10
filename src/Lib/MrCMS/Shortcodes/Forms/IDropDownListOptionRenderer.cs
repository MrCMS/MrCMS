using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public interface IDropDownListOptionRenderer
    {
        TagBuilder GetOption(FormListOption option, string existingValue);
    }
}