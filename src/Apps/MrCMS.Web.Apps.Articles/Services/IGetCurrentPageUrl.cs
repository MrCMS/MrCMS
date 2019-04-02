using System.Linq;
using Microsoft.AspNetCore.Routing;
using MrCMS.Services;

//Todo: potentially move this in core if best way
namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IGetCurrentPageUrl
    {
        string GetUrl(object queryString = null);
    }

    public class GetCurrentPageUrl : IGetCurrentPageUrl
    {
        private readonly IGetCurrentPage _getCurrentPage;
        private readonly IGetLiveUrl _getLiveUrl;

        public GetCurrentPageUrl(IGetCurrentPage getCurrentPage, IGetLiveUrl getLiveUrl)
        {
            _getCurrentPage = getCurrentPage;
            _getLiveUrl = getLiveUrl;
        }

        public string GetUrl(object queryString = null)
        {
            var page = _getCurrentPage.GetPage();
            var url = _getLiveUrl.GetUrlSegment(page, true);

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
