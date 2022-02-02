using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class GetPageData
    {
        private readonly IGetWebpageForPath _getWebpageForPath;
        private readonly ICanPreviewWebpage _canPreview;
        private readonly IWebpageMetadataService _webpageMetadataService;

        public GetPageData(IGetWebpageForPath getWebpageForPath, ICanPreviewWebpage canPreview,
            IWebpageMetadataService webpageMetadataService)
        {
            _getWebpageForPath = getWebpageForPath;
            _canPreview = canPreview;
            _webpageMetadataService = webpageMetadataService;
        }

    }
}