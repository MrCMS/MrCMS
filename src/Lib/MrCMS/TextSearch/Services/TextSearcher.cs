using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.TextSearch.Entities;
using MrCMS.TextSearch.EntityConverters;
using NHibernate;
using X.PagedList;

namespace MrCMS.TextSearch.Services
{
    public class TextSearcher : ITextSearcher
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly IStatelessSession _session;
        private readonly IServiceProvider _serviceProvider;

        public TextSearcher(ICurrentSiteLocator currentSiteLocator, IStatelessSession session,
            IServiceProvider serviceProvider)
        {
            _currentSiteLocator = currentSiteLocator;
            _session = session;
            _serviceProvider = serviceProvider;
        }

        public async Task<IList<TextSearchItem>> Search(ITextSearcher.TextSearcherQuery query)
        {
            return await BuildQuery(query).Take(query.ResultSize).ToListAsync();
        }

        public async Task<IPagedList<TextSearchItem>> SearchPaged(ITextSearcher.PagedTextSearcherQuery query)
        {
            return await BuildQuery(query).PagedAsync(query.Page, query.ResultSize);
        }

        public List<Type> GetTypes()
        {
            // we're loading all the known converters and getting their base types
            return TextSearchConverterMap.Types.Values.Distinct().Select(x => _serviceProvider.GetRequiredService(x))
                .OfType<BaseTextSearchEntityConverter>().Select(x => x.BaseType).Distinct().ToList();
        }

        private IQueryable<TextSearchItem> BuildQuery(ITextSearcher.TextSearcherQuery searcherQuery)
        {
            var currentSiteId = _currentSiteLocator.GetCurrentSite().Id;
            var query = _session.Query<TextSearchItem>()
                .Where(x => x.Text.Contains(searcherQuery.Term))
                .Where(x => x.SiteId == null || x.SiteId == currentSiteId);

            if (!string.IsNullOrWhiteSpace(searcherQuery.Type))
                query = query.Where(x => x.EntityType == searcherQuery.Type);

            if (searcherQuery.CreatedOnFrom.HasValue)
                query = query.Where(x => x.EntityCreatedOn >= searcherQuery.CreatedOnFrom);
            if (searcherQuery.CreatedOnTo.HasValue)
                query = query.Where(x => x.EntityCreatedOn <= searcherQuery.CreatedOnTo);

            if (searcherQuery.UpdatedOnFrom.HasValue)
                query = query.Where(x => x.EntityUpdatedOn >= searcherQuery.UpdatedOnFrom);
            if (searcherQuery.UpdatedOnTo.HasValue)
                query = query.Where(x => x.EntityUpdatedOn <= searcherQuery.UpdatedOnTo);

            query = searcherQuery.SortBy switch
            {
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.UpdatedOnDesc => query
                    .OrderByDescending(x => x.EntityUpdatedOn),
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.UpdatedOn => query.OrderBy(x => x.EntityUpdatedOn),
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.CreatedOnDesc => query
                    .OrderByDescending(x => x.EntityCreatedOn),
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.CreatedOn => query.OrderBy(x => x.EntityCreatedOn),
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.DisplayName => query.OrderBy(x => x.DisplayName),
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.DisplayNameDesc => query
                    .OrderByDescending(x => x.DisplayName),
                _ => throw new ArgumentOutOfRangeException()
            };

            return query;
        }
    }
}