using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class GetPageData : IGetPageData
    {
        private readonly IGetWebpageForPath _getWebpageForPath;
        private readonly ICanPreviewWebpage _canPreview;

        public GetPageData(IGetWebpageForPath getWebpageForPath, ICanPreviewWebpage canPreview)
        {
            _getWebpageForPath = getWebpageForPath;
            _canPreview = canPreview;
        }

        public async Task<PageData> GetData(string url, string method)
        {
            Webpage webpage = await _getWebpageForPath.GetWebpage(url);
            if (webpage == null)
            {
                return null;
            }

            var isPreview = !webpage.Published;
            if (isPreview && !await _canPreview.CanPreview(webpage))
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
                Action = metadata.GetAction(method),
                Webpage = webpage
            };
        }
    }

}