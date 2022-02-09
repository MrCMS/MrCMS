using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class GetCurrentPageUrl : IGetCurrentPageUrl
    {
        private readonly IGetCurrentPage _getCurrentPage;
        private readonly IGetLiveUrl _getLiveUrl;

        public GetCurrentPageUrl(IGetCurrentPage getCurrentPage, IGetLiveUrl getLiveUrl)
        {
            _getCurrentPage = getCurrentPage;
            _getLiveUrl = getLiveUrl;
        }

        public async Task<string> GetUrl(object queryString = null)
        {
            var page = _getCurrentPage.GetPage();
            var url = await _getLiveUrl.GetUrlSegment(page, true);

            if (queryString != null)
            {
                url += string.Format("?{0}",
                    string.Join("&",
                        new RouteValueDictionary(queryString)
                            .Select(pair => $"{pair.Key}={pair.Value}")));
            }

            return url;
        }
    }
}