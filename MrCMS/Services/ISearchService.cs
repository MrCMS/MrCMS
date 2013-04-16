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
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Models;
using MrCMS.Paging;
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

    public class SearchService : ISearchService
    {
        private readonly ISearcher<Webpage, WebpageIndexDefinition> _webpageSearcher;
        private readonly ISearcher<Document, DocumentIndexDefinition> _documentSearcher;

        public SearchService(ISearcher<Webpage, WebpageIndexDefinition> webpageSearcher, ISearcher<Document, DocumentIndexDefinition> documentSearcher)
        {
            _webpageSearcher = webpageSearcher;
            _documentSearcher = documentSearcher;
        }

        public IEnumerable<SearchResultModel> SearchDocuments<T>(string searchTerm) where T : Document
        {
            return
                SearchResults<T>(searchTerm).Select
                    (x => new SearchResultModel
                    {
                        DocumentId = x.Id.ToString(),
                        Name = x.Name,
                        LastUpdated = x.UpdatedOn.ToShortDateString(),
                        DocumentType = x.DocumentType.BreakUpString(),
                        DisplayName = x.Name + " (" + x.DocumentType.BreakUpString() + ")",
                    });
        }

        private IPagedList<T> SearchResults<T>(string searchTerm, int? parentId = null, int page = 1, int pageSize = 10) where T : Document
        {
            var documentSearchQuery = new DocumentSearchQuery<T>(searchTerm, parentId);
            var searchResults = _documentSearcher.Search(documentSearchQuery.ToQuery(), page, pageSize);
            return new StaticPagedList<T>(searchResults.OfType<T>(), searchResults.GetMetaData());
        }

        public IPagedList<DetailedSearchResultModel> SearchDocumentsDetailed<T>(string searchTerm, int? parentId, int page = 1) where T : Document
        {
            var searchResults = SearchResults<T>(searchTerm, parentId, page);
            return GetDetailedSearchResultModel(searchResults, page);
        }

        public IPagedList<Webpage> SiteSearch(string query, int page = 1, int pageSize = 10)
        {
            var searchQuery = new WebpageSearchQuery(query);
            return _webpageSearcher.Search(searchQuery.ToQuery(), page, pageSize, searchQuery.GetFilter());
        }

        private IPagedList<DetailedSearchResultModel> GetDetailedSearchResultModel<T>(IPagedList<T> args, int page) where T : Document
        {
            return new StaticPagedList<DetailedSearchResultModel>(
                args.Select(arg => new DetailedSearchResultModel
                {
                    DocumentId = arg.Id.ToString(),
                    Name = arg.Name,
                    DisplayName = arg.Name + " (" + arg.DocumentType.BreakUpString() + ")",
                    LastUpdated = arg.UpdatedOn.ToShortDateString(),
                    DocumentType = arg.DocumentType.BreakUpString(),
                    CreatedOn = arg.CreatedOn.ToShortDateString(),
                    EditUrl =
                        string.Format("/Admin/{0}/Edit/{1}",
                                      GetDocumentEditType(arg),
                                      arg.Id),
                    ViewUrl =
                        (arg is Webpage)
                            ? "/" + (arg as Webpage).LiveUrlSegment
                            : null
                }), args.GetMetaData());
        }

        private string GetDocumentEditType(Document document)
        {
            switch (document.Unproxy().GetType().Name)
            {
                case "Layout":
                    return "Layout";
                case "MediaCategory":
                    return "MediaCategory";
                default:
                    return "Webpage";
            }
        }
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

        public Query ToQuery()
        {
            var booleanQuery = new BooleanQuery();
            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var multiPhraseQuery = new MultiPhraseQuery();
                foreach (var s in _searchTerm.Split(' '))
                    multiPhraseQuery.Add(new Term(DocumentIndexDefinition.Name.FieldName, s));
                //booleanQuery.Add(new FuzzyQuery(new Term("name", _searchTerm), 0.3f, 2), Occur.SHOULD)
                booleanQuery.Add(multiPhraseQuery, Occur.SHOULD);
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
                                                                      FieldDefinition.GetFields(
                                                                          WebpageIndexDefinition.Name,
                                                                          WebpageIndexDefinition.BodyContent,
                                                                          WebpageIndexDefinition.MetaTitle,
                                                                          WebpageIndexDefinition.MetaKeywords,
                                                                          WebpageIndexDefinition.MetaDescription),
                                                                      new StandardAnalyzer(Version.LUCENE_30));
                query = multiFieldQueryParser.Parse(_searchTerm + "~");
            }
            var queryString = query.ToString();
            Debug.WriteLine(queryString);
            return query;
        }

        public Filter GetFilter()
        {
            var dateValue = DateTools.DateToString(DateTime.UtcNow, DateTools.Resolution.SECOND);
            var filter = FieldCacheRangeFilter.NewStringRange(WebpageIndexDefinition.PublishOn.FieldName, null,
                                                              dateValue, false, true);
            return filter;
        }
    }
}