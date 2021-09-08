using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;
using System;

namespace MrCMS.Shortcodes.Forms
{
    public class ElementRendererManager : IElementRendererManager
    {
        private readonly IServiceProvider _serviceProvider;

        public ElementRendererManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFormElementRenderer GetPropertyRenderer<T>(T property) where T : FormProperty
        {
            var type = typeof(IFormElementRenderer<>).MakeGenericType(property.GetType());
            return _serviceProvider.GetService(type) as IFormElementRenderer;
        }

        public TagBuilder GetPropertyContainer(FormRenderingType formRenderingType, FormProperty property)
        {
            if (formRenderingType == FormRenderingType.Bootstrap3 || formRenderingType == FormRenderingType.Bootstrap4)
            {
                // if (property is TextBox || property is TextArea || property is DropDownList || property is FileUpload || property is Email)
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