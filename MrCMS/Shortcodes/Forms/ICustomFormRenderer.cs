using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Shortcodes.Forms
{
    public interface ICustomFormRenderer
    {
        string GetForm(IHtmlHelper helper, Webpage webpage, FormSubmittedStatus submittedStatus);
    }
}