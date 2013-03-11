using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Shortcodes
{
    public class DefaultFormRenderer : IDefaultFormRenderer
    {
        private readonly IElementRendererManager _elementRendererManager;

        public DefaultFormRenderer(IElementRendererManager elementRendererManager)
        {
            _elementRendererManager = elementRendererManager;
        }

        public string GetDefault(Webpage webpage)
        {
            if (webpage == null)
                return string.Empty;
            foreach (var property in webpage.FormProperties)
            {
                var formElementRenderer = _elementRendererManager.GetElementRenderer(property);
                formElementRenderer.AppendLabel(property);
                formElementRenderer.AppendElement(property);
            }
            return string.Empty;
        }
    }

    public interface IElementRendererManager
    {
        IFormElementRenderer<T> GetElementRenderer<T>(T property) where T : FormProperty;
    }

    public class ElementRendererManager : IElementRendererManager
    {
        public IFormElementRenderer<T> GetElementRenderer<T>(T property) where T : FormProperty
        {
            return MrCMSApplication.Get<IFormElementRenderer<T>>();
        }
    }
}