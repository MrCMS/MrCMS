using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class GetPageData : IGetPageData
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

        public PageData GetData(string url, string method)
        {
            return GetData(_getWebpageForPath.GetWebpage(url), method);
        }

        public PageData GetData(Webpage webpage, string method)
        {
            if (webpage == null)
            {
                return null;
            }

            var isPreview = !webpage.Published;
            if (isPreview && !_canPreview.CanPreview(webpage))
            {
                return null;
            }

            var metadata = _documentMetadataService.GetMetadata(webpage);

            return new PageData
            {
                IsPreview = isPreview,
                Id = webpage.Id,
                Type = webpage.GetType(),
                Controller = metadata.GetController(method),
                Action = metadata.GetAction(method),
                Webpage = webpage
            };
        }
    }
}