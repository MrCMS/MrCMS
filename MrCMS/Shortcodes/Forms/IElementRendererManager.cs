using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public interface IElementRendererManager
    {
        IFormElementRenderer GetElementRenderer<T>(T property) where T : FormProperty;
    }
}