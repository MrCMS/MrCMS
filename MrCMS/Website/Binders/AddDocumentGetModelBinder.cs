using System.Web.Mvc;
using MrCMS.Services;
using NHibernate;
using Ninject;

namespace MrCMS.Website.Binders
{
    public class AddDocumentGetModelBinder : DocumentModelBinder
    {
        public AddDocumentGetModelBinder(IKernel kernel, IDocumentService documentService)
            : base(kernel, documentService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
            return model;
        }
    }
}