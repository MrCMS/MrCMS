using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Settings;
using System;
using System.Linq;
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
            var type = typeof(IFormElementRenderer<>).MakeGenericType(property.GetType());
            var concreteType = TypeHelper.GetAllConcreteTypesAssignableFrom(type).FirstOrDefault();
            if (concreteType != null)
            {
                return _serviceProvider.GetService(concreteType) as IFormElementRenderer;
            }

            return null;
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