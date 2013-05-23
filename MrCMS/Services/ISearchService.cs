using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Website;
using Document = MrCMS.Entities.Documents.Document;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Services
{
    public interface ISearchService
    {
        IEnumerable<SearchResultModel> SearchDocuments<T>(string searchTerm) where T : Document;
        IPagedList<DetailedSearchResultModel> SearchDocumentsDetailed<T>(string searchTerm, int? parentId, int page = 1) where T : Document;
        IPagedList<Webpage> SiteSearch(string query, int page = 1, int pageSize = 10);
    }

    internal class DocumentSearchQuery<T> where T : Document
    {
        private readonly string _searchTerm;
        private readonly int? _parentId;

        public DocumentSearchQuery(string searchTerm, int? parentId = null)
        {
            _searchTerm = searchTerm;
            _parentId = parentId;
        }
        
        private string MakeFuzzy(string keywords)
        {
            var split = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", split.Select(s => s + "~"));
        }

        public Query ToQuery()
        {
            var booleanQuery = new BooleanQuery();
            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var fuzzySearchTerm = MakeFuzzy(_searchTerm);
                var q = new MultiFieldQueryParser(Version.LUCENE_30, FieldDefinition.GetFieldNames(
                                                                          DocumentIndexDefinition.Name,
                                                                          DocumentIndexDefinition.UrlSegment), new StandardAnalyzer(Version.LUCENE_30));
                var query = q.Parse(fuzzySearchTerm);

                booleanQuery.Add(query, Occur.MUST);

            };
            if (typeof(Webpage).IsAssignableFrom(typeof(T)))
                booleanQuery.Add(new TermQuery(new Term("type", typeof(T).FullName)), Occur.MUST);
            if (typeof(Layout).IsAssignableFrom(typeof(T)))
                booleanQuery.Add(new TermQuery(new Term("type", "Layout")), Occur.MUST);
            if (typeof(MediaCategory).IsAssignableFrom(typeof(T)))
                booleanQuery.Add(new TermQuery(new Term("type", "MediaCategory")), Occur.MUST);
            if (_parentId.HasValue)
                booleanQuery.Add(
                    new TermQuery(new Term(DocumentIndexDefinition.ParentId.FieldName, _parentId.ToString())), Occur.MUST);
            return booleanQuery.Clauses.Any() ? (Query)booleanQuery : new MatchAllDocsQuery();
        }
    }
    internal class WebpageSearchQuery
    {
        private string MakeFuzzy(string keywords, decimal? fuzzyness = null)
        {
            var split = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", split.Select(s => s + "~"+(fuzzyness.HasValue ? fuzzyness : null)));
        }

        private readonly string _searchTerm;

        public WebpageSearchQuery(string searchTerm)
        {
            _searchTerm = searchTerm;
        }

        public Query ToQuery()
        {
            Query query = new MatchAllDocsQuery();
            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {

                var multiFieldQueryParser = new MultiFieldQueryParser(Version.LUCENE_30,
                                                                      FieldDefinition.GetFieldNames(
                                                                          WebpageIndexDefinition.Name,
                                                                          WebpageIndexDefinition.BodyContent,
                                                                          WebpageIndexDefinition.MetaTitle,
                                                                          WebpageIndexDefinition.MetaKeywords,
                                                                          WebpageIndexDefinition.MetaDescription),
                                                                      new StandardAnalyzer(Version.LUCENE_30));
                query = multiFieldQueryParser.Parse(MakeFuzzy(_searchTerm, 0.5m));
            }
            var queryString = query.ToString();
            Debug.WriteLine(queryString);
            return query;
        }

        public Filter GetFilter()
        {
            var dateValue = DateTools.DateToString(CurrentRequestData.Now, DateTools.Resolution.SECOND);
            var filter = FieldCacheRangeFilter.NewStringRange(WebpageIndexDefinition.PublishOn.FieldName, null,
                                                              dateValue, false, true);
            return filter;
        }
    }
}