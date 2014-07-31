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
            var typeInfo = GetTypeByName(controllerContext);
            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, typeInfo.Item1), typeInfo.Item1);

            var webpage = base.BindModel(controllerContext, bindingContext) as Webpage;

            //set include as navigation as default 
            if (webpage != null)
            {
                if (typeInfo.Item2.HasValue)
                {
                    webpage.PageTemplate = Session.Get<PageTemplate>(typeInfo.Item2.Value);
                }
                webpage.RevealInNavigation = webpage.GetMetadata().RevealInNavigation;
            }

            return webpage;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
            Type modelType)
        {
            Type type = GetTypeByName(controllerContext).Item1;
            return Activator.CreateInstance(type);
        }

        private static Tuple<Type,int?> GetTypeByName(ControllerContext controllerContext)
        {
            string valueFromContext = GetValueFromContext(controllerContext, "DocumentType");
            var strings = valueFromContext.Split(new[]{"-"},StringSplitOptions.RemoveEmptyEntries);
            if (strings.Count() == 1)
                return
                    new Tuple<Type, int?>(TypeHelper.MappedClasses.FirstOrDefault(x => x.FullName == valueFromContext),
                        null);
            int id;
            return new Tuple<Type, int?>(TypeHelper.MappedClasses.FirstOrDefault(x => x.FullName == strings[0]),
                int.TryParse(strings[1], out id) ? id : (int?) null);
        }
    }
}