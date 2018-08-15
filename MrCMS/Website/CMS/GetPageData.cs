using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class GetPageData : IGetPageData
    {
        private readonly IGetWebpageForPath _getWebpageForPath;
        private readonly ISetCurrentPage _setCurrentPage;
        private readonly ICanPreviewWebpage _canPreview;

        public GetPageData(IGetWebpageForPath getWebpageForPath, ISetCurrentPage setCurrentPage, ICanPreviewWebpage canPreview)
        {
            _getWebpageForPath = getWebpageForPath;
            _setCurrentPage = setCurrentPage;
            _canPreview = canPreview;
        }

        public PageData GetData(string url, string method)
        {
            Webpage webpage = _getWebpageForPath.GetWebpage(url);
            if (webpage == null)
            {
                return null;
            }

            var isPreview = !webpage.Published;
            if (isPreview && !_canPreview.CanPreview(webpage))
            {
                return null;
            }

            var metadata = webpage.GetMetadata();

            return new PageData
            {
                IsPreview = isPreview,
                Id = webpage.Id,
                Type = webpage.GetType(),
                Controller = metadata.GetController(method),
                Action = metadata.GetAction(method)
            };
        }
    }

}