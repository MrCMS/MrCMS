using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes
{
    public interface ICustomFormRenderer
    {
        string GetForm(Webpage webpage);
    }
}