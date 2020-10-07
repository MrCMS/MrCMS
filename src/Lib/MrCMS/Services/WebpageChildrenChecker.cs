using System.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class WebpageChildrenChecker : IWebpageChildrenChecker
    {
        private readonly IDocumentMetadataService _documentMetadataService;

        public WebpageChildrenChecker(IDocumentMetadataService documentMetadataService)
        {
            _documentMetadataService = documentMetadataService;
        }
        public bool CanAddChildren(Webpage webpage)
        {
            return _documentMetadataService.GetMetadata(webpage).ValidChildrenTypes.Any();
        }
    }
}