using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public class SiteUrlResolver : ISiteUrlResolver
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SiteUrlResolver(ICurrentSiteLocator currentSiteLocator, IHttpContextAccessor httpContextAccessor)
        {
            _currentSiteLocator = currentSiteLocator;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentSiteUrl()
        {
            return GetSiteUrl(_currentSiteLocator.GetCurrentSite());
        }

        public string GetSiteUrl(Site site)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            string scheme = request?.IsHttps == true
                ? "https://"
                : "http://";
            string authority;
            var hostValue = request?.Host.Value;
            authority = (hostValue == site.StagingUrl ? site.StagingUrl : site.BaseUrl);
            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');

            return $"{scheme}{authority}/";
        }
    }
}