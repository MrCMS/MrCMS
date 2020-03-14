using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.Search;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class UniversalSearchIndexSearcher : IUniversalSearchIndexSearcher
    {
        private readonly ISearchConverter _searchConverter;
        private readonly IDataReader _dataReader;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public UniversalSearchIndexSearcher(IUniversalSearchIndexManager universalSearchIndexManager,
            ISearchConverter searchConverter, IDataReader dataReader, IConfigurationProvider configurationProvider)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
            _searchConverter = searchConverter;
            _dataReader = dataReader;
            _configurationProvider = configurationProvider;
        }

        public async Task<List<UniversalSearchItemQuickSearch>> QuickSearch(QuickSearchParams searchParams)
        {
            IndexSearcher searcher = await _universalSearchIndexManager.GetSearcher();
            var query = new BooleanQuery();
            AddFilter(query, searchParams.Term);

            if (!string.IsNullOrWhiteSpace(searchParams.Type))
            {
                query.Add(FilterByEntityType(searchParams.Type));
            }

            var topDocs = searcher.Search(query, 10);

            List<UniversalSearchItem> universalSearchItems =
                topDocs.ScoreDocs.Select(doc => _searchConverter.Convert(searcher.Doc(doc.Doc))).ToList();

            return universalSearchItems.Select(item => new UniversalSearchItemQuickSearch(item)).ToList();
        }

        public async Task<IPagedList<AdminSearchResult>> Search(AdminSearchQuery searchQuery)
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            int pageSize = siteSettings.DefaultPageSize;
            //using (MiniProfiler.Current.Step("Search for results"))
            {
                IndexSearcher searcher = await _universalSearchIndexManager.GetSearcher();
                var query = new BooleanQuery();
                AddFilter(query, searchQuery.Term);
                if (!string.IsNullOrWhiteSpace(searchQuery.Type))
                {
                    query.Add(FilterByEntityType(searchQuery.Type));
                }

                if (searchQuery.CreatedOnFrom.HasValue || searchQuery.CreatedOnTo.HasValue)
                {
                    query.Add(GetDateQuery(searchQuery));
                }
                TopDocs topDocs = GetTopDocs(searchQuery, searcher, query, pageSize);

                List<UniversalSearchItem> universalSearchItems =
                    GetUniversalSearchItems(searchQuery, topDocs, pageSize, searcher);


                //using (MiniProfiler.Current.Step("Get Results"))
                var adminSearchResults = universalSearchItems.Select(item =>
                {
                    Type systemType = TypeHelper.GetTypeByName(item.SystemType);
                    var entity = _dataReader.Get(systemType, item.Id).GetAwaiter().GetResult() as SystemEntity; // todo - refactor out sync call
                    return new AdminSearchResult(item, entity);
                }).ToList();

                return new StaticPagedList<AdminSearchResult>(adminSearchResults, searchQuery.Page, pageSize,
                    topDocs.TotalHits);
            }
        }

        private static void AddFilter(BooleanQuery query, string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
            {
                query.Add(term.GetSearchFilterByTerm(UniversalSearchFieldNames.PrimarySearchTerms,
                    UniversalSearchFieldNames.SecondarySearchTerms, UniversalSearchFieldNames.DisplayName,
                    UniversalSearchFieldNames.Id, UniversalSearchFieldNames.EntityType));
            }
        }

        private static TopDocs GetTopDocs(AdminSearchQuery searchQuery, IndexSearcher searcher, BooleanQuery query, int pageSize)
        {
            //using (MiniProfiler.Current.Step("Get Top Docs"))
            return searcher.Search(query, pageSize * searchQuery.Page);
        }

        private List<UniversalSearchItem> GetUniversalSearchItems(AdminSearchQuery searchQuery, TopDocs topDocs, int pageSize, IndexSearcher searcher)
        {
            //using (MiniProfiler.Current.Step("Get Search Items"))
            return topDocs.ScoreDocs.Skip(pageSize * (searchQuery.Page - 1))
                .Select(doc => _searchConverter.Convert(searcher.Doc(doc.Doc)))
                .ToList();
        }

        private BooleanClause GetDateQuery(AdminSearchQuery searchQuery)
        {
            return new BooleanClause(new TermRangeQuery(UniversalSearchFieldNames.CreatedOn,
                searchQuery.CreatedOnFrom.HasValue ? new BytesRef(DateTools.DateToString(searchQuery.CreatedOnFrom.Value, DateTools.Resolution.SECOND)) : null,
                searchQuery.CreatedOnTo.HasValue ? new BytesRef(DateTools.DateToString(searchQuery.CreatedOnTo.Value, DateTools.Resolution.SECOND)) : null,
                searchQuery.CreatedOnFrom.HasValue,
                searchQuery.CreatedOnTo.HasValue
                ), Occur.MUST);
        }

        private static BooleanClause FilterByEntityType(string type)
        {
            return new BooleanClause(
                new TermQuery(new Term(UniversalSearchFieldNames.EntityType, type)), Occur.MUST);
        }

    }
}