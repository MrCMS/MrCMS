using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;

namespace MrCMS.Shortcodes.Forms
{
    public interface IElementRendererManager
    {
        IFormElementRenderer GetElementRenderer<T>(T property) where T : FormProperty;
        TagBuilder GetElementContainer (FormRenderingType formRendererType, FormProperty property);
    }
}