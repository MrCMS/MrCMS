using System;
using MrCMS.Entities.Documents.Web;
using NHibernate;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Website.Caching;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class GetHomePage : IGetHomePage
    {
        private readonly IStatelessSession _statelessSession;
        private readonly ISession _session;
        private readonly ICacheManager _cacheManager;
        public static string CurrentHomePageCacheKey = "current.home-page";
        private readonly ICurrentSiteLocator _currentSiteLocator;

        public GetHomePage(IStatelessSession statelessSession, ISession session, ICacheManager cacheManager,
            ICurrentSiteLocator currentSiteLocator)
        {
            _statelessSession = statelessSession;
            _session = session;
            _cacheManager = cacheManager;
            _currentSiteLocator = currentSiteLocator;
        }

        public async Task<Webpage> Get()
        {
            var id = await GetId(_currentSiteLocator.GetCurrentSite());
            return await _session.GetAsync<Webpage>(id);
        }

        public async Task<Webpage> GetForSite(Site site)
        {
            var id = await GetId(site);
            return await _session.GetAsync<Webpage>(id);
        }

        private async Task<int> GetId(Site site)
        {
            var siteId = site.Id;
            return await _cacheManager.GetOrCreateAsync($"{CurrentHomePageCacheKey}.{siteId}", async () =>
            {
                var pageId = await _statelessSession.Query<Webpage>()
                    .Where(document => document.Parent == null && document.Published && !document.IsDeleted)
                    .Where(x => x.Site.Id == siteId)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
                return pageId;
            }, TimeSpan.FromMinutes(10), CacheExpiryType.Absolute);
        }
    }
}