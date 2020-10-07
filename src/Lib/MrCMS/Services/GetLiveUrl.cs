using System;
using System.Text;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class GetLiveUrl : IGetLiveUrl
    {
        private readonly IGetHomePage _getHomePage;
        private readonly SiteSettings _settings;

        public GetLiveUrl(IGetHomePage getHomePage, SiteSettings settings)
        {
            _getHomePage = getHomePage;
            _settings = settings;
        }
        public string GetUrlSegment(Webpage webpage, bool addLeadingSlash)
        {
            var builder = new StringBuilder(addLeadingSlash ? "/" : string.Empty);
            builder.Append(GetSegment(webpage));
            return builder.ToString();
        }

        public string GetAbsoluteUrl(Webpage webpage)
        {
            if (webpage == null)
                return null;
            string scheme = (webpage.RequiresSSL|| _settings.SSLEverywhere)
                ? "https://"
                : "http://";
            string authority = webpage.Site.BaseUrl;
            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');

            return $"{scheme}{authority}/{GetSegment(webpage)}";
        }

        private string GetSegment(Webpage webpage)
        {
            if (webpage == null)
            {
                return string.Empty;
            }

            var homepage = _getHomePage.Get();
            return webpage.Id == homepage?.Id ? string.Empty : webpage.UrlSegment;
        }
    }
}