using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class CurrentSiteLocator : ICurrentSiteLocator
    {
        private readonly IGlobalRepository<Site> _siteRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private Site _currentSite;

        public CurrentSiteLocator(IGlobalRepository<Site> siteRepository, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _siteRepository = siteRepository;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }

        public async Task<Site> GetCurrentSite()
        {
            return _currentSite ??= await GetSiteFromSettingForDebugging() ?? GetSiteFromRequest();
        }

        private async Task<Site> GetSiteFromSettingForDebugging()
        {
            var appSetting = _configuration["debugSiteId"];

            return int.TryParse(appSetting, out var id) ? await _siteRepository.GetData(id) : null;
        }

        private Site GetSiteFromRequest()
        {
            var authority = _contextAccessor.HttpContext.Request.Host.ToString();

            var allSites = _siteRepository.Query().ToList();
            var site = allSites.FirstOrDefault(s => s.BaseUrl != null && (s.BaseUrl.Equals(authority, StringComparison.OrdinalIgnoreCase) || (s.StagingUrl != null && s.StagingUrl.Equals(authority, StringComparison.OrdinalIgnoreCase))));
            if (site == null)
            {
                var redirectedDomains = allSites.SelectMany(s => s.RedirectedDomains).ToList();
                var redirectedDomain =
                    redirectedDomains.FirstOrDefault(
                        s => s.Url != null && s.Url.Equals(authority, StringComparison.OrdinalIgnoreCase));
                if (redirectedDomain != null)
                    site = redirectedDomain.Site;
            }

            return site ?? allSites.First();
        }
    }
}