using MrCMS.Services;
using Ninject;

namespace MrCMS.Website.Binders
{
    public abstract class DocumentModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IDocumentService _documentService;

        protected DocumentModelBinder(IKernel kernel, IDocumentService documentService)
            : base(kernel)
        {
            _documentService = documentService;
        }

        protected IDocumentService DocumentService
        {
            get { return _documentService; }
        }
    }
}