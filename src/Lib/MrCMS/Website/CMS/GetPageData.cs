using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class GetPageData
    {
        private readonly IGetWebpageForPath _getWebpageForPath;
        private readonly ICanPreviewWebpage _canPreview;
        private readonly IDocumentMetadataService _documentMetadataService;

        public GetPageData(IGetWebpageForPath getWebpageForPath, ICanPreviewWebpage canPreview,
            IDocumentMetadataService documentMetadataService)
        {
            _getWebpageForPath = getWebpageForPath;
            _canPreview = canPreview;
            _documentMetadataService = documentMetadataService;
        }

    }
}