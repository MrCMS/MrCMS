using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using NHibernate;
using System.Linq;
using Ninject;

namespace MrCMS.Website.Binders
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