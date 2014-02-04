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
        T SaveDocument<T>(T document) where T : Document;
        IEnumerable<T> GetAllDocuments<T>() where T : Document;
        void DeleteDocument<T>(T document) where T : Document;

        UrlHistory GetHistoryItemByUrl(string url);

        bool ExistAny(Type type);
        IEnumerable<T> GetDocumentsByParent<T>(T parent) where T : Document;
        T GetDocumentByUrl<T>(string url) where T : Document;
        string GetDocumentUrl(string pageName, Webpage parent, bool useHierarchy = false);
        Layout GetDefaultLayout(Webpage currentPage);
        void SetTags(string taglist, Document document);
        void SetOrder(int documentId, int order);
        void SetOrders(List<SortItem> items);
        bool AnyPublishedWebpages();
        bool AnyWebpages();
        IEnumerable<Webpage> GetWebPagesByParentIdForNav(int parentId);
        void SetParent(Document document, int? parentId);

        DocumentMetadata GetDefinitionByType(Type type);

        void SetFrontEndRoles(string frontEndRoles, Webpage webpage);
        IEnumerable<SelectListItem> GetValidParents(Webpage webpage);
        IEnumerable<Document> GetParents(int? parent);
        Webpage GetHomePage();

        void HideWidget(Webpage document, int widgetId);
        void ShowWidget(Webpage document, int widgetId);

        void PublishNow(Webpage document);
        void Unpublish(Webpage document);

        bool UrlIsValidForMediaCategory(string urlSegment, int? id);
        bool UrlIsValidForLayout(string urlSegment, int? id);
        bool UrlIsValidForWebpage(string url, int? id);
        bool UrlIsValidForWebpageUrlHistory(string url);

        RedirectResult RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage;
        T GetUniquePage<T>() where T : Document, IUniquePage;
        
        DocumentVersion GetDocumentVersion(int id);
        void RevertToVersion(DocumentVersion documentVersion);
    }
}