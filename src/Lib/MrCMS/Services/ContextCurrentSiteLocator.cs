using System;
using System.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class ContextCurrentSiteLocator : ICurrentSiteLocator
    {
        private readonly IRepository<Site> _siteRepository;
        private readonly IRepository<RedirectedDomain> _redirectedDomainRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private Site _currentSite;

        public ContextCurrentSiteLocator(IRepository<Site> siteRepository,
            IRepository<RedirectedDomain> redirectedDomainRepository,
            IHttpContextAccessor contextAccessor)
        {
            _siteRepository = siteRepository;
            _redirectedDomainRepository = redirectedDomainRepository;
            _contextAccessor = contextAccessor;
        }

        public Site GetCurrentSite()
        {
            return _currentSite ??= GetSiteFromSettingForDebugging() ?? GetSiteFromRequest();
        }

        public RedirectedDomain GetCurrentRedirectedDomain()
        {
            var authority = GetAuthority();
            var allDomains = _redirectedDomainRepository.Query()
                .Fetch(x => x.Site)
                .ToList();
            var domain = allDomains.FirstOrDefault(s =>
                s.Url != null && s.Url.Equals(authority, StringComparison.OrdinalIgnoreCase));
            return domain;
        }

        private Site GetSiteFromSettingForDebugging()
        {
            var appSetting = ConfigurationManager.AppSettings["debugSiteId"];

            int id;
            return int.TryParse(appSetting, out id)
                ? _siteRepository.Query().FirstOrDefault(x => x.Id == id)
                : null;
        }

        private Site GetSiteFromRequest()
        {
            var authority = GetAuthority();

            var allSites = _siteRepository.Query().ToList();
            var site = allSites.FirstOrDefault(s =>
                s.BaseUrl != null && (s.BaseUrl.Equals(authority, StringComparison.OrdinalIgnoreCase) ||
                                      s.StagingUrl != null &&
                                      s.StagingUrl.Equals(authority, StringComparison.OrdinalIgnoreCase)));
            // if (site == null)
            // {
            //     var redirectedDomains = allSites.SelectMany(s => s.RedirectedDomains).Select(x => x.Unproxy()).ToList();
            //     var redirectedDomain =
            //         redirectedDomains.FirstOrDefault(
            //             s => s.Url != null && s.Url.Equals(authority, StringComparison.OrdinalIgnoreCase));
            //     if (redirectedDomain != null)
            //         site = redirectedDomain.Site;
            // }

            return site; // ?? allSites.First();
        }

        private string GetAuthority()
        {
            return _contextAccessor.HttpContext.Request.Host.ToString();
        }
    }
}