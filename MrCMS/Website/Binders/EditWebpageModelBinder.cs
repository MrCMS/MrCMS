using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using NHibernate;
using Ninject;

namespace MrCMS.Website.Binders
{
    public class EditWebpageModelBinder : WebpageModelBinder
    {
        public EditWebpageModelBinder(IKernel kernel, IDocumentService documentService) : base(kernel, documentService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var document = base.BindModel(controllerContext, bindingContext) as Document;

            if (document is Webpage)
            {
                var frontEndRoles = GetValueFromContext(controllerContext, "FrontEndRoles");
                DocumentService.SetFrontEndRoles(frontEndRoles, document as Webpage);
            }

            return document;
        }
    }
}