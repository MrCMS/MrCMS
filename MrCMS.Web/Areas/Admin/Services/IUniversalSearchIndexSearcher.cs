using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Utils;
using MrCMS.Paging;
using MrCMS.Search;
using MrCMS.Search.Models;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Models.Search;
using NHibernate;
using StackExchange.Profiling;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IUniversalSearchIndexSearcher
    {
        List<UniversalSearchItemQuickSearch> QuickSearch(QuickSearchParams searchParams);
        IPagedList<AdminSearchResult> Search(AdminSearchQuery searchQuery);
    }

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
            using (IndexSearcher searcher = _universalSearchIndexManager.GetSearcher())
            {
                var query = new BooleanQuery { GetFilterByTerm(searchParams.Term) };

                if (!string.IsNullOrWhiteSpace(searchParams.Type))
                {
                    query.Add(FilterByEntityType(searchParams.Type));
                }
                TopDocs topDocs = searcher.Search(query, 10);

                List<UniversalSearchItem> universalSearchItems =
                    topDocs.ScoreDocs.Select(doc => _searchConverter.Convert(searcher.Doc(doc.Doc))).ToList();

                return universalSearchItems.Select(item => new UniversalSearchItemQuickSearch(item)).ToList();
            }
        }

        public IPagedList<AdminSearchResult> Search(AdminSearchQuery searchQuery)
        {
            int pageSize = _siteSettings.DefaultPageSize;
            using (MiniProfiler.Current.Step("Search for results"))
            using (IndexSearcher searcher = _universalSearchIndexManager.GetSearcher())
            {
                var query = new BooleanQuery();
                if (!string.IsNullOrWhiteSpace(searchQuery.Term))
                {
                    query.Add(GetFilterByTerm(searchQuery.Term));
                }
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
                using (MiniProfiler.Current.Step("Get Results"))
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

        private static TopDocs GetTopDocs(AdminSearchQuery searchQuery, IndexSearcher searcher, BooleanQuery query, int pageSize)
        {
            using (MiniProfiler.Current.Step("Get Top Docs"))
            return searcher.Search(query, pageSize * searchQuery.Page);
        }

        private List<UniversalSearchItem> GetUniversalSearchItems(AdminSearchQuery searchQuery, TopDocs topDocs, int pageSize, IndexSearcher searcher)
        {
            using (MiniProfiler.Current.Step("Get Search Items"))
                return topDocs.ScoreDocs.Skip(pageSize * (searchQuery.Page - 1))
                    .Select(doc => _searchConverter.Convert(searcher.Doc(doc.Doc)))
                    .ToList();
        }

        private BooleanClause GetDateQuery(AdminSearchQuery searchQuery)
        {
            return new BooleanClause(new TermRangeQuery(UniversalSearchFieldNames.CreatedOn,
                searchQuery.CreatedOnFrom.HasValue ? DateTools.DateToString(searchQuery.CreatedOnFrom.Value, DateTools.Resolution.SECOND) : null,
                searchQuery.CreatedOnTo.HasValue ? DateTools.DateToString(searchQuery.CreatedOnTo.Value, DateTools.Resolution.SECOND) : null,
                searchQuery.CreatedOnFrom.HasValue,
                searchQuery.CreatedOnTo.HasValue
                ), Occur.MUST);
        }

        private static BooleanClause FilterByEntityType(string type)
        {
            return new BooleanClause(
                new TermQuery(new Term(UniversalSearchFieldNames.EntityType, type)), Occur.MUST);
        }

        private static BooleanClause GetFilterByTerm(string term)
        {
            var analyser = new StandardAnalyzer(Version.LUCENE_30);
            var parser = new MultiFieldQueryParser(Version.LUCENE_30,
                new[]
                {
                    UniversalSearchFieldNames.PrimarySearchTerms, UniversalSearchFieldNames.SecondarySearchTerms,
                    UniversalSearchFieldNames.DisplayName, UniversalSearchFieldNames.Id,
                    UniversalSearchFieldNames.EntityType
                }, analyser);
            var booleanClause = new BooleanClause(term.SafeGetSearchQuery(parser, analyser), Occur.MUST);
            return booleanClause;
        }
    }

    public class UniversalSearchItemQuickSearch
    {
        private readonly UniversalSearchItem _item;

        public UniversalSearchItemQuickSearch(UniversalSearchItem item)
        {
            _item = item;
        }

        public string id
        {
            get { return _item.SearchGuid.ToString(); }
        }

        public string value
        {
            get { return _item.DisplayName; }
        }

        public string actionUrl
        {
            get { return _item.ActionUrl; }
        }

        public string systemType
        {
            get { return _item.SystemType; }
        }

        public string displayType
        {
            get
            {
                Type typeByName = TypeHelper.GetTypeByName(_item.SystemType);
                return typeByName == null ? "" : typeByName.Name.BreakUpString();
            }
        }

        public string systemId
        {
            get { return _item.Id.ToString(); }
        }
    }
}