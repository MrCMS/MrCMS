using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class AddWebpageModelBinder : WebpageModelBinder
    {
        public AddWebpageModelBinder(IKernel kernel, IDocumentTagsAdminService documentTagsAdminService)
            : base(kernel, documentTagsAdminService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type type = GetTypeByName(controllerContext);
            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var webpage = base.BindModel(controllerContext, bindingContext) as Webpage;

            //set include as navigation as default 
            if (webpage != null)
            {
                webpage.RevealInNavigation = webpage.GetMetadata().RevealInNavigation;
            }

            return webpage;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
            Type modelType)
        {
            Type type = GetTypeByName(controllerContext);
            return Activator.CreateInstance(type);
        }

        private static Type GetTypeByName(ControllerContext controllerContext)
        {
            string valueFromContext = GetValueFromContext(controllerContext, "DocumentType");
            return TypeHelper.MappedClasses.FirstOrDefault(x => x.FullName == valueFromContext);
        }
    }
}