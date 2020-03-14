using System;
using System.Text;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class GetLiveUrl : IGetLiveUrl
    {
        private readonly IGetHomePage _getHomePage;
        private readonly IConfigurationProvider _configurationProvider;

        public GetLiveUrl(IGetHomePage getHomePage, IConfigurationProvider configurationProvider)
        {
            _getHomePage = getHomePage;
            _configurationProvider = configurationProvider;
        }
        public async Task<string> GetUrlSegment(Webpage webpage, bool addLeadingSlash = false)
        {
            var builder = new StringBuilder(addLeadingSlash ? "/" : string.Empty);
            builder.Append(await GetSegment(webpage));
            return builder.ToString();
        }

        public async Task<string> GetAbsoluteUrl(Webpage webpage)
        {
            if (webpage == null)
                return null;
            var settings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            string scheme = (webpage.RequiresSSL|| settings.SSLEverywhere)
                ? "https://"
                : "http://";
            string authority = webpage.Site.BaseUrl;
            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');

            return string.Format("{0}{1}/{2}", scheme, authority, GetSegment(webpage));
        }

        private async Task<string> GetSegment(Webpage webpage)
        {
            if (webpage == null)
            {
                return string.Empty;
            }

            var homepage =await _getHomePage.Get();
            return webpage.Id == homepage?.Id ? string.Empty : webpage.UrlSegment;
        }
    }
}