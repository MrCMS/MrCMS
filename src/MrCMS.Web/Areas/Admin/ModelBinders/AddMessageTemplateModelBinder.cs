using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class MessageTemplateOverrideModelBinder : IModelBinder
    {
        private static Type GetTypeByName(ModelBindingContext modelBindingContext)
        {
            var valueFromContext = modelBindingContext.ValueProvider.GetValue("TemplateType");
            return TypeHelper.GetTypeByName(valueFromContext.FirstValue);
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(bindingContext);

            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();
            var metadata = metadataProvider.GetMetadataForType(type);
            bindingContext.ModelMetadata = metadata;
            var binderFactory = serviceProvider.GetRequiredService<IModelBinderFactory>();

            var modelBinder = binderFactory.CreateBinder(new ModelBinderFactoryContext
            {
                Metadata = metadata,
                BindingInfo = BindingInfo.GetBindingInfo(Enumerable.Empty<object>(), metadata),
            });
            return modelBinder.BindModelAsync(bindingContext);
        }
    }
}