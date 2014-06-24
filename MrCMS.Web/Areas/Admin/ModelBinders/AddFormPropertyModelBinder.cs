using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class AddFormPropertyModelBinder : MrCMSDefaultModelBinder
    {
        public AddFormPropertyModelBinder(IKernel kernel) : base(kernel)
        {
        }

        private static Type GetTypeByName(ControllerContext controllerContext)
        {
            return
                TypeHelper.GetAllConcreteTypesAssignableFrom<FormProperty>()
                          .FirstOrDefault(type => type.Name == GetValueFromContext(controllerContext, "type"));
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(controllerContext);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var formProperty = base.BindModel(controllerContext, bindingContext) as FormProperty;

            if (formProperty != null && formProperty.Webpage != null)
                formProperty.Webpage.FormProperties.Add(formProperty);
            return formProperty;
        }
    }
}