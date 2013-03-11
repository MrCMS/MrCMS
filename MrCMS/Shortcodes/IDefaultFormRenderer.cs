using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes
{
    public interface IDefaultFormRenderer
    {
        string GetDefault(Webpage webpage);
    }
}