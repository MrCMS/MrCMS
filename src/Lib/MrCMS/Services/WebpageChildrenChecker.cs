using System.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class WebpageChildrenChecker : IWebpageChildrenChecker
    {
        private readonly IWebpageMetadataService _webpageMetadataService;

        public WebpageChildrenChecker(IWebpageMetadataService webpageMetadataService)
        {
            _webpageMetadataService = webpageMetadataService;
        }
        public bool CanAddChildren(Webpage webpage)
        {
            return _webpageMetadataService.GetMetadata(webpage).ValidChildrenTypes.Any();
        }
    }
}