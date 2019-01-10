using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Shortcodes.Forms
{
    public interface IDefaultFormRenderer
    {
        IHtmlContent GetDefault(IHtmlHelper helper, Form form, FormSubmittedStatus submittedStatus);
    }
}