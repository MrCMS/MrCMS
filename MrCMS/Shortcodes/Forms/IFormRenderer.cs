using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes.Forms
{
    public interface IFormRenderer
    {
        string RenderForm(Webpage webpage, FormSubmittedStatus submitted);
    }
}