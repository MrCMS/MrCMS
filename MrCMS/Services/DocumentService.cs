using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly CurrentSite _currentSite;

        public DocumentService(ISession session, SiteSettings siteSettings, CurrentSite currentSite)
        {
            _session = session;
            _siteSettings = siteSettings;
            _currentSite = currentSite;
        }

        public void AddDocument<T>(T document) where T : Document
        {
            var sameParentDocs =
                GetDocumentsByParent(document.Parent as T);
            document.DisplayOrder = sameParentDocs.Any() ? sameParentDocs.Max(doc => doc.DisplayOrder) + 1 : 0;
            document.CustomInitialization(this, _session);
            _session.Transact(session => session.SaveOrUpdate(document));
        }

        public T GetDocument<T>(int id) where T : Document
        {
            return _session.Get<T>(id);
        }

        public T GetUniquePage<T>()
            where T : Document, IUniquePage
        {
            return _session.QueryOver<T>().Where(arg => arg.Site == _currentSite.Site).Take(1).Cacheable().SingleOrDefault();
        }

        public T SaveDocument<T>(T document) where T : Document
        {
            _session.Transact(session =>
                                  {
                                      document.OnSaving(session);
                                      session.SaveOrUpdate(document);
                                  });
            return document;
        }

        public IEnumerable<T> GetAllDocuments<T>() where T : Document
        {
            return _session.QueryOver<T>().Where(arg => arg.Site == _currentSite.Site).Cacheable().List();
        }

        public bool ExistAny(Type type)
        {
            var uniqueResult =
                _session.CreateCriteria(type)
                        .Add(Restrictions.Eq(Projections.Property("Site"), _currentSite.Site))
                        .SetProjection(Projections.RowCount())
                        .UniqueResult<int>();
            return uniqueResult != 0;
        }

        public IEnumerable<T> GetFrontEndDocumentsByParentId<T>(int? id) where T : Document
        {
            IEnumerable<T> children;
            Document document = null;
            if (id.HasValue)
            {
                document = _session.Get<Document>(id);
                children = document.Children.Select(TypeHelper.Unproxy).OfType<T>();
            }
            else
            {
                children = _session.QueryOver<T>().Where(arg => arg.Parent == null).List();
            }

            children =
                children.Where(
                    arg => !(arg is Webpage) || (arg as Webpage).IsAllowed(CurrentRequestData.CurrentUser));

            if (document != null)
            {
                var documentTypeDefinition = document.GetMetadata();
                if (documentTypeDefinition != null)
                {
                    return Sort(documentTypeDefinition, children);
                }
            }
            return children.OrderBy(arg => arg.DisplayOrder);
        }

        public IEnumerable<T> GetDocumentsByParent<T>(T parent) where T : Document
        {
            IEnumerable<T> children =
                _session.QueryOver<T>().Where(arg => arg.Parent == parent && arg.Site == _currentSite.Site).List();

            if (parent != null)
            {
                var documentTypeDefinition = parent.GetMetadata();
                if (documentTypeDefinition != null)
                {
                    return Sort(documentTypeDefinition, children);
                }
            }
            return children.OrderBy(arg => arg.DisplayOrder);
        }

        public IEnumerable<T> GetAdminDocumentsByParent<T>(T parent) where T : Document
        {
            IEnumerable<T> children =
                _session.QueryOver<T>().Where(arg => arg.Parent == parent && arg.Site == _currentSite.Site).List();

            if (parent is Webpage)
            {
                var documentTypeDefinition = parent.GetMetadata();
                if (documentTypeDefinition != null)
                {
                    return Sort(documentTypeDefinition, children);
                }
            }
            return children.OrderBy(arg => arg.DisplayOrder);
        }

        private static IEnumerable<T> Sort<T>(DocumentMetadata documentMetadata, IEnumerable<T> children) where T : Document
        {
            var childrenSortedNull =
                children.OrderByDescending(arg => documentMetadata.SortBy(arg) == null);
            return documentMetadata.SortByDesc
                       ? childrenSortedNull.ThenByDescending(documentMetadata.SortBy)
                       : childrenSortedNull.ThenBy(documentMetadata.SortBy);
        }


        public T GetDocumentByUrl<T>(string url) where T : Document
        {
            return
                _session.QueryOver<T>()
                        .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                        .Take(1)
                        .SingleOrDefault();
        }

        public string GetDocumentUrl(string pageName, Webpage parent, bool useHierarchy = false)
        {
            var stringBuilder = new StringBuilder();

            if (useHierarchy)
            {
                //get breadcrumb from parent
                if (parent != null)
                {
                    stringBuilder.Insert(0, SeoHelper.TidyUrl(parent.UrlSegment) + "/");
                }
            }
            //add page name

            stringBuilder.Append(SeoHelper.TidyUrl(pageName));

            //make sure the URL is unique

            if (GetDocumentByUrl<Webpage>(stringBuilder.ToString()) != null)
            {
                var counter = 1;

                while (GetDocumentByUrl<Webpage>(string.Format("{0}-{1}", stringBuilder, counter)) != null)
                    counter++;

                stringBuilder.AppendFormat("-{0}", counter);
            }
            return stringBuilder.ToString();
        }

        public IEnumerable<SearchResultModel> SearchDocuments<T>(string searchTerm) where T : Document
        {
            return
                GetSearchResults<T>(searchTerm).Select
                    (x => new SearchResultModel
                              {
                                  DocumentId = x.Id.ToString(),
                                  Name = x.Name,
                                  LastUpdated = x.UpdatedOn.ToShortDateString(),
                                  DocumentType = x.DocumentType
                              });
        }

        private IList<T> GetSearchResults<T>(string searchTerm, int? parentId = null) where T : Document
        {
            return SearchResults<T>(searchTerm, parentId);
        }

        private IList<T> SearchResults<T>(string searchTerm, int? parentId) where T : Document
        {
            var queryOver = _session.QueryOver<T>().Where(x => x.Site == _currentSite.Site && x.Name.IsLike(searchTerm, MatchMode.Anywhere));
            if (parentId.HasValue)
            {
                queryOver = queryOver.Where(arg => arg.Parent.Id == parentId);
            }
            return queryOver.List();
        }

        public IPagedList<DetailedSearchResultModel> SearchDocumentsDetailed<T>(string searchTerm, int? parentId, int page = 1) where T : Document
        {
            var searchResults = GetSearchResults<T>(searchTerm, parentId);
            return GetDetailedSearchResultModel(searchResults, page);
        }

        public IPagedList<SearchResult> SiteSearch(string query, int? page, int pageSize = 10)
        {
            page = page ?? 1;
            if (string.IsNullOrWhiteSpace(query))
            {
                return new StaticPagedList<SearchResult>(new SearchResult[0], page.Value, pageSize, 0);

            }
            return
                _session.QueryOver<Webpage>().Where(x => x.Published && x.Name.IsLike(query, MatchMode.Anywhere)).Paged(
                    page.Value, pageSize, webpages => webpages.Select(webpage =>
                                                                new SearchResult
                                                                    {
                                                                        Name = webpage.Name,
                                                                        Url = webpage.LiveUrlSegment,
                                                                        PublishedOn =
                                                                            webpage.PublishOn.GetValueOrDefault()
                                                                    }));
        }

        private IPagedList<DetailedSearchResultModel> GetDetailedSearchResultModel<T>(IEnumerable<T> args, int page) where T : Document
        {
            return new PagedList<DetailedSearchResultModel>(
                args.Select(arg => new DetailedSearchResultModel
                                       {
                                           DocumentId = arg.Id.ToString(),
                                           Name = arg.Name,
                                           LastUpdated = arg.UpdatedOn.ToShortDateString(),
                                           DocumentType = arg.DocumentType,
                                           CreatedOn = arg.CreatedOn.ToShortDateString(),
                                           EditUrl =
                                               string.Format("/Admin/{0}/Edit/{1}",
                                                             GetDocumentEditType(arg),
                                                             arg.Id),
                                           ViewUrl =
                                               (arg is Webpage)
                                                   ? "/" + (arg as Webpage).LiveUrlSegment
                                                   : null
                                       }), page, 10);
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

        public Layout GetDefaultLayout(Webpage currentPage)
        {
            if (currentPage != null)
            {
                string defaultLayoutName = currentPage.GetMetadata().DefaultLayoutName;
                if (!String.IsNullOrEmpty(defaultLayoutName))
                {
                    var layout = _session.QueryOver<Layout>().Where(x => x.Name == defaultLayoutName).Cacheable().Take(1).SingleOrDefault();
                    if (layout != null)
                        return layout;
                }
            }
            var settingValue = _siteSettings.DefaultLayoutId;

            return _session.Get<Layout>(settingValue) ??
                   _session.QueryOver<Layout>()
                           .Where(layout => layout.Site == currentPage.Site)
                           .Take(1)
                           .Cacheable()
                           .SingleOrDefault();
        }

        public void SetTags(string taglist, Document document)
        {
            if (document == null) throw new ArgumentNullException("document");

            if (taglist == null)
                taglist = string.Empty;

            var tagNames =
                taglist.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x));

            var existingTags = document.Tags.ToList();

            tagNames.ForEach(name =>
                                 {
                                     var tag = GetTag(name) ?? new Tag { Name = name };
                                     _session.SaveOrUpdate(tag);
                                     if (!document.Tags.Contains(tag))
                                     {
                                         document.Tags.Add(tag);
                                         tag.Documents.Add(document);
                                     }
                                     existingTags.Remove(tag);
                                 });

            existingTags.ForEach(tag =>
                                     {
                                         document.Tags.Remove(tag);
                                         tag.Documents.Remove(document);
                                         _session.SaveOrUpdate(tag);
                                     });
            _session.SaveOrUpdate(document);
        }

        private Tag GetTag(string name)
        {
            return _session.QueryOver<Tag>().Where(tag => tag.Name.IsLike(name, MatchMode.Exact)).Take(1).SingleOrDefault();
        }

        public void SetOrder(int documentId, int order)
        {
            _session.Transact(session =>
                                  {
                                      var document = session.Get<Document>(documentId);
                                      document.DisplayOrder = order;
                                      session.SaveOrUpdate(document);
                                  });
        }

        public void SetOrders(List<SortItem> items)
        {
            _session.Transact(session => items.ForEach(item =>
                                                           {
                                                               var document = session.Get<Document>(item.Id);
                                                               document.DisplayOrder = item.Order;
                                                               session.Update(document);
                                                           }));
        }

        public bool AnyPublishedWebpages()
        {
            return _session.QueryOver<Webpage>().Where(webpage => webpage.Published).Cacheable().RowCount() > 0;
        }

        public bool AnyWebpages()
        {
            return _session.QueryOver<Webpage>().Cacheable().RowCount() > 0;
        }

        public IEnumerable<Webpage> GetWebPagesByParentIdForNav(int parentId)
        {
            return
                _session.QueryOver<Webpage>().Where(
                    x => x.Parent.Id == parentId && x.Published && x.PublishOn < DateTime.Now && x.RevealInNavigation).
                    List();
        }

        public void DeleteDocument<T>(T document) where T : Document
        {
            if (document != null)
            {
                _session.Transact(session =>
                                      {
                                          document.OnDeleting(session);
                                          session.Delete(document);
                                      });
            }
        }

        public void PublishNow(Webpage document)
        {
            if (document.PublishOn == null)
            {
                document.PublishOn = DateTime.Now;
                SaveDocument(document);
            }
        }

        public void Unpublish(Webpage document)
        {
            document.PublishOn = null;
            SaveDocument(document);
        }

        public void HideWidget(Webpage document, int widgetId)
        {
            var widget = _session.Get<Widget>(widgetId);

            if (document == null || widget == null) return;

            if (document.ShownWidgets.Contains(widget))
                document.ShownWidgets.Remove(widget);
            else if (!document.HiddenWidgets.Contains(widget))
                document.HiddenWidgets.Add(widget);
            SaveDocument(document);
        }

        public void ShowWidget(Webpage document, int widgetId)
        {
            var widget = _session.Get<Widget>(widgetId);

            if (document == null || widget == null) return;

            if (document.HiddenWidgets.Contains(widget))
                document.HiddenWidgets.Remove(widget);
            else if (!document.ShownWidgets.Contains(widget))
                document.ShownWidgets.Add(widget);
            SaveDocument(document);

        }

        public Document Get404Page()
        {
            var error404Id = _siteSettings.Error404PageId;

            return _session.Get<Document>(error404Id)
                   ?? GetDocumentByUrl<Webpage>("404")
                   ?? MrCMSApplication.PublishedRootChildren().OfType<Document>().FirstOrDefault();
        }

        public Document Get500Page()
        {
            var error500Id = _siteSettings.Error500PageId;

            return _session.Get<Document>(error500Id)
                   ?? GetDocumentByUrl<Webpage>("500")
                   ?? MrCMSApplication.PublishedRootChildren().OfType<Document>().FirstOrDefault();
        }

        public DocumentVersion GetDocumentVersion(int id)
        {
            return _session.Get<DocumentVersion>(id);
        }

        public void SetParent(Document document, int? parentId)
        {
            if (document == null) return;

            var existingParent = document.Parent;
            var parent = parentId.HasValue ? GetDocument<Webpage>(parentId.Value) : null;

            document.Parent = parent;
            if (parent != null)
            {
                parent.Children.Add(document);
                SaveDocument(parent);
            }
            if (existingParent != null)
            {
                existingParent.Children.Remove(document);
                SaveDocument(existingParent);
            }
        }

        public DocumentMetadata GetDefinitionByType(Type type)
        {
            return DocumentMetadataHelper.GetMetadataByType(type);
        }
    }
}