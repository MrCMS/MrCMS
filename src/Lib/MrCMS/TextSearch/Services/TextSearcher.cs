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
using NHibernate.Criterion;
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
            return await BuildQuery(query).Take(query.ResultSize).ListAsync();
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

        private IQueryOver<TextSearchItem, TextSearchItem> BuildQuery(ITextSearcher.TextSearcherQuery searcherQuery)
        {
            var currentSiteId = _currentSiteLocator.GetCurrentSite().Id;
            var query = _session.QueryOver<TextSearchItem>()
                .Where(x => x.Text.IsInsensitiveLike(searcherQuery.Term, MatchMode.Anywhere))
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
                    .OrderBy(x => x.EntityUpdatedOn)
                    .Desc,
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.UpdatedOn => query.OrderBy(x => x.EntityUpdatedOn)
                    .Asc,
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.CreatedOnDesc => query
                    .OrderBy(x => x.EntityCreatedOn)
                    .Desc,
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.CreatedOn => query.OrderBy(x => x.EntityCreatedOn)
                    .Asc,
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.DisplayName => query.OrderBy(x => x.DisplayName)
                    .Asc,
                ITextSearcher.TextSearcherQuery.TextSearcherQuerySort.DisplayNameDesc => query
                    .OrderBy(x => x.DisplayName)
                    .Desc,
                _ => throw new ArgumentOutOfRangeException()
            };

            return query;
        }
    }
}