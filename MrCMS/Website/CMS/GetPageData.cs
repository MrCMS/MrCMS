using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class GetPageData : IGetPageData
    {
        private readonly IGetDocumentByUrl<Webpage> _getWebpageByUrl;
        private readonly IGetHomePage _getHomePage;
        private readonly ISetCurrentPage _setCurrentPage;
        private readonly ICanPreviewWebpage _canPreview;

        public GetPageData(IGetDocumentByUrl<Webpage> getWebpageByUrl, IGetHomePage getHomePage, ISetCurrentPage setCurrentPage, ICanPreviewWebpage canPreview)
        {
            _getWebpageByUrl = getWebpageByUrl;
            _getHomePage = getHomePage;
            _setCurrentPage = setCurrentPage;
            _canPreview = canPreview;
        }


        public PageData GetData(string url, string method)
        {
            Webpage webpage = string.IsNullOrWhiteSpace(url)
                ? _getHomePage.Get()
                : _getWebpageByUrl.GetByUrl(url);
            if (webpage == null)
                return null;

            _setCurrentPage.SetPage(webpage);

            var isPreview = !webpage.Published;
            if (isPreview && !_canPreview.CanPreview(webpage))
                return null;

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