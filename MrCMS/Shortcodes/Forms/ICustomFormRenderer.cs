using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes.Forms
{
    public interface ICustomFormRenderer
    {
        string GetForm(Webpage webpage, FormSubmittedStatus submittedStatus);
    }
}