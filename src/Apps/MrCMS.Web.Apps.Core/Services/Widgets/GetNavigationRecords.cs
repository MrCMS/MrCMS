using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Website.Caching;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetNavigationRecords : IGetNavigationRecords
    {
        private readonly ISession _session;
        private readonly IGetLiveUrl _getLiveUrl;
        private readonly ICacheManager _cacheManager;
        private readonly ICurrentSiteLocator _currentSiteLocator;
        public const string NavigationCacheKey = "NavigationCaheKey";

        public GetNavigationRecords(ISession session, IGetLiveUrl getLiveUrl, ICacheManager cacheManager,
            ICurrentSiteLocator currentSiteLocator)
        {
            _session = session;
            _getLiveUrl = getLiveUrl;
            _cacheManager = cacheManager;
            _currentSiteLocator = currentSiteLocator;
        }

        public async Task<NavigationList> GetRootNavigation(bool includeChildren)
        {
            var siteId = _currentSiteLocator.GetCurrentSite().Id;
            return await _cacheManager.GetOrCreateAsync(
                $"GetNavigationRecords.GetRootNavigation.{siteId}.{includeChildren}",
                async () =>
                {
                    var rootPages = await GetPages(null);
                    var childPages = includeChildren ? await GetPages(rootPages) : new List<NavigationDto>();
                    var navigationRecords =
                        rootPages
                            .Select(webpage => new NavigationRecord
                            (webpage.Name, webpage.Url,
                                webpage.GetType(), childPages.Where(y => y.ParentId == webpage.Id)
                                    .Select(child =>
                                        new NavigationRecord(child.Name, child.Url,
                                            child.PageType)
                                    ).ToList()));

                    var navigationList = new NavigationList(navigationRecords.ToList());
                    return navigationList;
                }, TimeSpan.FromMinutes(10), CacheExpiryType.Absolute);
        }

        private async Task<IList<NavigationDto>> GetPages(IList<NavigationDto> parents)
        {
            var queryOver = _session.Query<Webpage>();
            if (parents == null)
            {
                queryOver = queryOver.Where(webpage => webpage.Parent == null);
            }
            else
            {
                var parentIds = parents.Select(p => p.Id).ToList();
                queryOver = queryOver.Where(webpage => parentIds.Contains(webpage.Parent.Id));
            }

            var pages = await queryOver.Where(webpage => webpage.RevealInNavigation && webpage.Published)
                .OrderBy(webpage => webpage.DisplayOrder).ToListAsync();

            return pages.Select(x => new NavigationDto
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                Url = x.UrlSegment.GetRelativeUrl(),
                PageType = x.GetType()
            }).ToList();
        }

        private class NavigationDto
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public int ParentId { get; init; }
            public string Url { get; init; }
            public Type PageType { get; init; }
        }
    }
}