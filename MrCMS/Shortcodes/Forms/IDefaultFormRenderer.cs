using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Shortcodes.Forms
{
    public interface IDefaultFormRenderer
    {
        string GetDefault(IHtmlHelper helper, Webpage webpage, FormSubmittedStatus submittedStatus);
    }
}