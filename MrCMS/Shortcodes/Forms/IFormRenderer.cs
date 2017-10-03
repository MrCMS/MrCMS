using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Shortcodes.Forms
{
    public interface IFormRenderer
    {
        string RenderForm(IHtmlHelper helper, Webpage webpage, FormSubmittedStatus submitted);
    }
}