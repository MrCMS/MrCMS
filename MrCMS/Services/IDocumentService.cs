using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Paging;
using NHibernate;

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

        
        DocumentVersion GetDocumentVersion(int id);
        void RevertToVersion(DocumentVersion documentVersion);
    }

    public interface IUniquePageService
    {
        RedirectResult RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage;
        T GetUniquePage<T>() where T : Document, IUniquePage;
    }

    public class UniquePageService : IUniquePageService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public UniquePageService(ISession session,Site site)
        {
            _session = session;
            _site = site;
        }

        public T GetUniquePage<T>()
            where T : Document, IUniquePage
        {
            return
                _session.QueryOver<T>()
                        .Where(arg => arg.Site.Id == _site.Id)
                        .Take(1)
                        .Cacheable()
                        .SingleOrDefault();
        }

        public RedirectResult RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage
        {
            var page = GetUniquePage<T>();
            var url = page != null ? string.Format("/{0}", page.LiveUrlSegment) : "/";
            if (routeValues != null)
            {
                var dictionary = new RouteValueDictionary(routeValues);
                url += string.Format("?{0}",
                                     string.Join("&",
                                                 dictionary.Select(
                                                     pair => string.Format("{0}={1}", pair.Key, pair.Value))));
            }
            return new RedirectResult(url);
        }

    }
}