using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;
namespace MrCMS.Shortcodes.Forms
{
    public class ElementRendererManager : IElementRendererManager
    {
        private readonly IServiceProvider _serviceProvider;

        public ElementRendererManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFormElementRenderer GetElementRenderer<T>(T property) where T : FormProperty
        {
            return _serviceProvider.GetService(typeof(IFormElementRenderer<>).MakeGenericType(property.GetType())) as IFormElementRenderer;
        }

        public TagBuilder GetElementContainer(FormRenderingType formRendererType, FormProperty property)
        {
            if (formRendererType == FormRenderingType.Bootstrap3)
            {
                if (property is TextBox || property is TextArea || property is DropDownList || property is FileUpload)
                {
                    var elementContainer = new TagBuilder("div");
                    elementContainer.AddCssClass("form-group");
                    return elementContainer;
                }
            }
            return null;
        }
    }
}