using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class AddMessageTemplateGetModelBinder : MessageTemplateModelBinder
    {
        public AddMessageTemplateGetModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
            return model;
        }
    }

    public class MessageTemplateOverrideModelBinder : MessageTemplateModelBinder
    {
        public MessageTemplateOverrideModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(controllerContext);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var messageTemplate = base.BindModel(controllerContext, bindingContext) as MessageTemplateBase;

            return messageTemplate;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var type = GetTypeByName(controllerContext);
            return Activator.CreateInstance(type);
        }

        private static Type GetTypeByName(ControllerContext controllerContext)
        {
            var valueFromContext = GetValueFromContext(controllerContext, "TemplateType");
            return TypeHelper.GetTypeByName(valueFromContext);
        }
    }
}