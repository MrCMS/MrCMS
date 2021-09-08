using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Website.Caching;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class GetDocumentByUrl<T> : IGetDocumentByUrl<T> where T : Document
    {
        private readonly ISession _session;
        private readonly ICacheManager _cacheManager;
        private readonly ICurrentSiteLocator _currentSiteLocator;

        public GetDocumentByUrl(ISession session, ICacheManager cacheManager, ICurrentSiteLocator currentSiteLocator)
        {
            _session = session;
            _cacheManager = cacheManager;
            _currentSiteLocator = currentSiteLocator;
        }

        public async Task<T> GetByUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            var id = await GetId(url);
            if (!id.HasValue)
                return null;
            return await _session.GetAsync<T>(id.Value);
        }

        private async Task<int?> GetId(string url)
        {
            var siteId = _currentSiteLocator.GetCurrentSite().Id;
            return await _cacheManager.GetOrCreateAsync($"get-by-url-{siteId}-{url}", async () =>
            {
                return await _session.Query<T>()
                    .WithOptions(x => x.SetCacheable(false))
                    .Where(doc => doc.UrlSegment == url)
                    .Select(x => x.Id).FirstOrDefaultAsync();
            }, TimeSpan.FromMinutes(10), CacheExpiryType.Sliding);
        }
    }
}