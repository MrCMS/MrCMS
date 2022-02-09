using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Website.Caching;
using NHibernate;

namespace MrCMS.Settings
{
    public class ConfigurationProviderFactory : IConfigurationProviderFactory
    {
        private readonly IStatelessSession _session;
        private readonly ICacheManager _cacheManager;
        private IEventContext _eventContext;

        public ConfigurationProviderFactory(IStatelessSession session, ICacheManager cacheManager)
        {
            _session = session;
            _cacheManager = cacheManager;
        }

        public IConfigurationProvider GetForSite(Site site)
        {
            return new SqlConfigurationProvider(_session, new KnownSiteLocator(site), _cacheManager, _eventContext);
        }
    }

    public class KnownSiteLocator : ICurrentSiteLocator
    {
        private readonly Site _site;

        public KnownSiteLocator(Site site)
        {
            _site = site;
        }

        public Site GetCurrentSite()
        {
            return _site;
        }

        public RedirectedDomain GetCurrentRedirectedDomain()
        {
            return null;
        }
    }
}