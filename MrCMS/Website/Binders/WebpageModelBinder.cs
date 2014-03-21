using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services;
using Ninject;

namespace MrCMS.Website.Binders
{
    public abstract class WebpageModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IDocumentService _documentService;

        protected WebpageModelBinder(IKernel kernel, IDocumentService documentService)
            : base(kernel)
        {
            _documentService = documentService;
        }

        protected IDocumentService DocumentService
        {
            get { return _documentService; }
        }

        public override object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var document = base.BindModel(controllerContext, bindingContext) as Document;
            var taglist = controllerContext.GetValueFromRequest("TagList") ?? string.Empty;
            _documentService.SetTags(taglist, document);
            return document;
        }
    }
}