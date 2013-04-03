using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Website;

namespace MrCMS.Shortcodes.Forms
{
    public class ElementRendererManager : IElementRendererManager
    {
        public IFormElementRenderer GetElementRenderer<T>(T property) where T : FormProperty
        {
            return MrCMSApplication.Get(typeof (IFormElementRenderer<>).MakeGenericType(property.GetType())) as IFormElementRenderer;
        }
    }
}