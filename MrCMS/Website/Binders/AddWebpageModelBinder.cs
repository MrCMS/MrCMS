using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using Ninject;

namespace MrCMS.Website.Binders
{
    public class AddWebpageModelBinder : WebpageModelBinder
    {
        public AddWebpageModelBinder(IKernel kernel, IDocumentService documentService)
            : base(kernel, documentService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(controllerContext);
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

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var type = GetTypeByName(controllerContext);
            return Activator.CreateInstance(type);
        }

        private static Type GetTypeByName(ControllerContext controllerContext)
        {
            string valueFromContext = GetValueFromContext(controllerContext, "DocumentType");
            return TypeHelper.MappedClasses.FirstOrDefault(x => x.FullName == valueFromContext);
        }
    }
}