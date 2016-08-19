using System;
using System.Configuration;
using System.Linq;
using System.Web;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class CurrentSiteLocator : ICurrentSiteLocator
    {
        private readonly IRepository<Site> _siteRepository;
        private readonly HttpRequestBase _requestBase;
        private Site _currentSite;
        public CurrentSiteLocator(IRepository<Site> siteRepository, HttpRequestBase requestBase)
        {
            _siteRepository = siteRepository;
            _requestBase = requestBase;
        }

        public Site GetCurrentSite()
        {
            return _currentSite ?? (_currentSite = GetSiteFromSettingForDebugging() ?? GetSiteFromRequest());
        }

        private Site GetSiteFromSettingForDebugging()
        {
            var appSetting = ConfigurationManager.AppSettings["debugSiteId"];

            int id;
            return int.TryParse(appSetting, out id) ? _siteRepository.Get(id) : null;
        }

        private Site GetSiteFromRequest()
        {
            var authority = _requestBase.Url.Authority;

            var allSites = _siteRepository.Query().ToList();
            var site = allSites.FirstOrDefault(s => s.BaseUrl != null && (s.BaseUrl.Equals(authority, StringComparison.OrdinalIgnoreCase) || (s.StagingUrl != null && s.StagingUrl.Equals(authority, StringComparison.OrdinalIgnoreCase))));
            if (site == null)
            {
                var redirectedDomains = allSites.SelectMany(s => s.RedirectedDomains).Select(x => x.Unproxy()).ToList();
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