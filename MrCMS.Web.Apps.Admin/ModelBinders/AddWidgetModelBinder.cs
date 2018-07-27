using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class AddWidgetModelBinder : IModelBinder
    {
        private static Type GetTypeByName(ModelBindingContext bindingContext)
        {
            return WidgetHelper.GetTypeByName(bindingContext.ValueProvider.GetValue("WidgetType").FirstValue);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(bindingContext);
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();
            bindingContext.ModelMetadata = metadataProvider.GetMetadataForType(type);
            var instance = ActivatorUtilities.CreateInstance(serviceProvider, typeof(SystemEntityBinder<>).MakeGenericType(type)) as IModelBinder;

            await instance.BindModelAsync(bindingContext);

            if (bindingContext.Result.Model is Widget widget)
            {
                widget.LayoutArea?.AddWidget(widget);
                widget.Webpage?.Widgets.Add(widget);
                if (bindingContext.ValueProvider.GetValue("AddType").FirstValue == "global")
                {
                    widget.Webpage = null;
                }
            }
        }
    }
}