using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Services
{
    public interface IDocumentService
    {
        void AddDocument<T>(T document) where T : Document;
        T GetDocument<T>(int id) where T : Document;
        T GetUniquePage<T>() where T : Document, IUniquePage;
        T SaveDocument<T>(T document) where T : Document;
        IEnumerable<T> GetAllDocuments<T>() where T : Document;
        bool ExistAny(Type type);
        IEnumerable<T> GetFrontEndDocumentsByParentId<T>(int? id) where T : Document;
        IEnumerable<T> GetDocumentsByParent<T>(T parent) where T : Document;
        IEnumerable<T> GetAdminDocumentsByParent<T>(T parent) where T : Document;
        T GetDocumentByUrl<T>(string url) where T : Document;
        UrlHistory GetHistoryItemByUrl(string url);
        string GetDocumentUrl(string pageName, Webpage parent, bool useHierarchy = false);
        Layout GetDefaultLayout(Webpage currentPage);
        void SetTags(string taglist, Document document);
        void SetOrder(int documentId, int order);
        void SetOrders(List<SortItem> items);
        bool AnyPublishedWebpages();
        bool AnyWebpages();
        IEnumerable<Webpage> GetWebPagesByParentIdForNav(int parentId);
        void DeleteDocument<T>(T document) where T : Document;
        void PublishNow(Webpage document);
        void Unpublish(Webpage document);
        void HideWidget(Webpage document, int widgetId);
        void ShowWidget(Webpage document, int widgetId);
        Document Get404Page();
        Document Get500Page();
        DocumentVersion GetDocumentVersion(int id);
        void SetParent(Document document, int? parentId);
        DocumentMetadata GetDefinitionByType(Type type);
        void SetFrontEndRoles(string frontEndRoles, Webpage webpage);
        void SetAdminRoles(string adminRoles, Webpage webpage);
        bool UrlIsValidForMediaCategory(string urlSegment, int? id);
        bool UrlIsValidForLayout(string urlSegment, int? id);
        bool UrlIsValidForWebpage(string url, int? id);
        bool UrlIsValidForWebpageUrlHistory(string url);
        IEnumerable<SelectListItem> GetValidParents(Webpage webpage);
    }
}