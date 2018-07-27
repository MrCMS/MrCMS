using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class MessageTemplateOverrideModelBinder : IModelBinder
    {
        //public MessageTemplateOverrideModelBinder(IKernel kernel)
        //    : base(kernel)
        //{
        //}

        //public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        //{
        //    var type = GetTypeByName(controllerContext);

        //    bindingContext.ModelMetadata =
        //        ModelMetadataProviders.Current.GetMetadataForType(
        //            () => CreateModel(controllerContext, bindingContext, type), type);

        //    var messageTemplate = base.BindModel(controllerContext, bindingContext) as MessageTemplate;

        //    return messageTemplate;
        //}

        //protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        //{
        //    var type = GetTypeByName(controllerContext);
        //    return Activator.CreateInstance(type);
        //}

        private static Type GetTypeByName(ModelBindingContext modelBindingContext)
        {
            var valueFromContext = modelBindingContext.ValueProvider.GetValue("TemplateType");
            return TypeHelper.GetTypeByName(valueFromContext.FirstValue);
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(bindingContext);

            var metadataProvider = bindingContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
            bindingContext.ModelMetadata = metadataProvider.GetMetadataForType(type);
                //ModelMetadataProviders.Current.GetMetadataForType(
                //    () => CreateModel(controllerContext, bindingContext, type), type);

            var modelBinder = new SimpleTypeModelBinder(type);
            return modelBinder.BindModelAsync(bindingContext);
        }
    }
}