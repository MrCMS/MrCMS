using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Helpers;
using MrCMS.Indexing.Querying;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Services
{
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
                            DocumentType = StringHelper.BreakUpString(x.DocumentType),
                            DisplayName = x.Name + " (" + StringHelper.BreakUpString(x.DocumentType) + ")",
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
                                : null,
                        AddChildUrl = (arg is Webpage) ?
                                          string.Format("/Admin/Webpage/Add/{0}",
                                                        arg.Id) : null,
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
}