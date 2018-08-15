using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Shortcodes.Forms
{
    public interface ICustomFormRenderer
    {
        IHtmlContent GetForm(IHtmlHelper helper, Webpage webpage, FormSubmittedStatus submittedStatus);
    }
}