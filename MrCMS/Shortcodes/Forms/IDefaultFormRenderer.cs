using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes.Forms
{
    public interface IDefaultFormRenderer
    {
        string GetDefault(Webpage webpage, FormSubmittedStatus submittedStatus);
    }
}