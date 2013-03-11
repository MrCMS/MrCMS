using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes
{
    public interface IFormRenderer
    {
        string RenderForm(Webpage webpage);
    }
}