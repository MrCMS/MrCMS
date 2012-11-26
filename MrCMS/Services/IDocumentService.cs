using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Services
{
    public interface IDocumentService
    {
        void AddDocument(Document document);
        T GetDocument<T>(int id) where T : Document;
        T SaveDocument<T>(T document) where T : Document;
        IEnumerable<T> GetAllDocuments<T>() where T : Document;
        IEnumerable<T> GetFrontEndDocumentsByParentId<T>(int? id) where T : Document;
        IEnumerable<T> GetAdminDocumentsByParentId<T>(int? id) where T : Document;
        T GetDocumentByUrl<T>(string url) where T : Document;
        string GetDocumentUrl(string pageName, int? parentId, bool useHierarchy = false);
        IEnumerable<SearchResultModel> SearchDocuments<T>(string searchTerm) where T : Document;
        IPagedList<DetailedSearchResultModel> SearchDocumentsDetailed<T>(string searchTerm, int? parentId, int page = 1) where T : Document;
        IPagedList<SearchResult> SiteSearch(string query, int? page, int pageSize = 10);
        Layout GetDefaultLayout(Webpage currentPage);
        int? GetLayoutId(int webpageId);
        void SetTags(string taglist, Document document);
        void SetOrder(int documentId, int order);
        void CreateDirectory(MediaCategory doc);
        bool AnyPublishedWebpages();
        bool AnyWebpages();
        IEnumerable<Webpage> GetWebPagesByParentIdForNav(int parentId);
        void DeleteDocument<T>(int id) where T : Document;
        void PublishNow(int documentId);
        void Unpublish(int documentId);
        void HideWidget(int id, int widgetId);
        void ShowWidget(int id, int widgetId);
        TextPage Get404Page();
        TextPage Get500Page();
        DocumentVersion GetDocumentVersion(int id);
        FormPosting GetFormPosting(int id);
    }
}