using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class GetLiveUrl : IGetLiveUrl
    {
        private readonly IGetHomePage _getHomePage;
        private readonly ISiteUrlResolver _siteUrlResolver;

        public GetLiveUrl(IGetHomePage getHomePage, ISiteUrlResolver siteUrlResolver)
        {
            _getHomePage = getHomePage;
            _siteUrlResolver = siteUrlResolver;
        }

        public async Task<string> GetUrlSegment(Webpage webpage, bool addLeadingSlash)
        {
            var builder = new StringBuilder(addLeadingSlash ? "/" : string.Empty);
            builder.Append(await GetSegment(webpage));
            return builder.ToString();
        }

        public async Task<string> GetAbsoluteUrl(Webpage webpage)
        {
            webpage = webpage.Unproxy();
            if (webpage == null)
                return null;

            var site = webpage.Site.Unproxy();

            var siteUrl = _siteUrlResolver.GetSiteUrl(site);

            return $"{siteUrl}{await GetSegment(webpage)}";
        }

        private async Task<string> GetSegment(Webpage webpage)
        {
            if (webpage == null)
            {
                return string.Empty;
            }

            var homepage = await _getHomePage.GetForSite(webpage.Site.Unproxy());
            return webpage.Id == homepage?.Id ? string.Empty : GetLiveUrl.GetSegmentEncoded(webpage);
        }

        private static string GetSegmentEncoded(Webpage webpage)
        {
            var parts = webpage.UrlSegment.Split("/");
            return string.Join("/", parts.Select(WebUtility.UrlEncode));
        }
    }
}