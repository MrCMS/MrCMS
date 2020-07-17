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
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class UniversalSearchIndexSearcher : IUniversalSearchIndexSearcher
    {
        private readonly ISearchConverter _searchConverter;
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public UniversalSearchIndexSearcher(IUniversalSearchIndexManager universalSearchIndexManager,
            ISearchConverter searchConverter, ISession session, SiteSettings siteSettings)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
            _searchConverter = searchConverter;
            _session = session;
            _siteSettings = siteSettings;
        }

        public List<UniversalSearchItemQuickSearch> QuickSearch(QuickSearchParams searchParams)
        {
            IndexSearcher searcher = _universalSearchIndexManager.GetSearcher();
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

        public IPagedList<AdminSearchResult> Search(AdminSearchQuery searchQuery)
        {
            int pageSize = _siteSettings.DefaultPageSize;
            //using (MiniProfiler.Current.Step("Search for results"))
            {
                IndexSearcher searcher = _universalSearchIndexManager.GetSearcher();
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


                List<AdminSearchResult> adminSearchResults;
                //using (MiniProfiler.Current.Step("Get Results"))
                adminSearchResults = universalSearchItems.Select(item =>
                {
                    Type systemType = TypeHelper.GetTypeByName(item.SystemType);
                    var entity = _session.Get(systemType, item.Id) as SystemEntity;
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