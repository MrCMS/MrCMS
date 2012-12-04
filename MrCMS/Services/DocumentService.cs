using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly FormService _formService;

        public DocumentService(ISession session, SiteSettings siteSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
        }

        public void AddDocument(Document document)
        {
            _session.Transact(session => session.SaveOrUpdate(document));
        }

        public T GetDocument<T>(int id) where T : Document
        {
            return _session.Get<T>(id);
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
            return _session.QueryOver<T>().Cacheable().List();
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
                    arg => !(arg is Webpage) || (arg as Webpage).IsAllowed(MrCMSApplication.CurrentUser));

            if (document != null)
            {
                var documentTypeDefinition = document.GetDefinition();
                if (documentTypeDefinition != null)
                {
                    return Sort(documentTypeDefinition, children);
                }
            }
            return children.OrderBy(arg => arg.DisplayOrder);
        }

        public IEnumerable<T> GetAdminDocumentsByParentId<T>(int? id) where T : Document
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
                    arg => !(arg is Webpage) || (arg as Webpage).IsAllowedForAdmin(MrCMSApplication.CurrentUser));

            if (document != null)
            {
                var documentTypeDefinition = document.GetDefinition();
                if (documentTypeDefinition != null)
                {
                    return Sort(documentTypeDefinition, children);
                }
            }
            return children.OrderBy(arg => arg.DisplayOrder);
        }

        private static IOrderedEnumerable<T> Sort<T>(DocumentTypeDefinition documentTypeDefinition, IEnumerable<T> children) where T : Document
        {
            var childrenSortedNull =
                children.OrderByDescending(
                    doc => doc.GetType().GetProperty(documentTypeDefinition.SortBy).GetValue(doc, null) == null);
            return documentTypeDefinition.SortByDesc
                       ? childrenSortedNull.ThenByDescending(
                           doc => doc.GetType().GetProperty(documentTypeDefinition.SortBy).GetValue(doc, null))
                       : childrenSortedNull.ThenBy(
                           doc => doc.GetType().GetProperty(documentTypeDefinition.SortBy).GetValue(doc, null));
        }


        public T GetDocumentByUrl<T>(string url) where T : Document
        {
            return _session.QueryOver<T>().Where(doc => doc.UrlSegment == url).Take(1).SingleOrDefault();
        }

        public string GetDocumentUrl(string pageName, int? parentId, bool useHierarchy = false)
        {
            var stringBuilder = new StringBuilder();

            if (useHierarchy)
            {
                //get breadcrumb from parent
                var parent = parentId.HasValue ? GetDocument<Document>(parentId.Value) : null;

                if (parent != null)
                {
                    stringBuilder.Insert(0, SeoHelper.TidyUrl(parent.LiveUrlSegment) + "/");
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

        private IList<T> GetSearchResults<T>(string searchTerm, int? parentId = null, int page = 1) where T : Document
        {
            return SearchResults<T>(searchTerm, parentId, page);
        }

        private IList<T> SearchResults<T>(string searchTerm, int? parentId, int page) where T : Document
        {
            var queryOver = _session.QueryOver<T>().Where(x => x.Name.IsLike(searchTerm, MatchMode.Anywhere));
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
                string defaultLayoutName = currentPage.GetDefinition().DefaultLayoutName;
                if (!String.IsNullOrEmpty(defaultLayoutName))
                {
                    var layout = _session.QueryOver<Layout>().Where(x => x.Name == defaultLayoutName).Cacheable().Take(1).SingleOrDefault();
                    if (layout != null)
                        return layout;

                }
            }
            var settingValue = _siteSettings.DefaultLayoutId;

            return _session.Get<Layout>(settingValue);
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
                                          document.OnDeleting();
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

        public void HideWidget(int id, int widgetId)
        {
            var document = GetDocument<Webpage>(id);
            var widget = _session.Get<Widget>(widgetId);

            if (document == null || widget == null) return;

            if (document.ShownWidgets.Contains(widget))
                document.ShownWidgets.Remove(widget);
            else if (!document.HiddenWidgets.Contains(widget))
                document.HiddenWidgets.Add(widget);
            SaveDocument(document);
        }

        public void ShowWidget(int id, int widgetId)
        {
            var document = GetDocument<Webpage>(id);
            var widget = _session.Get<Widget>(widgetId);

            if (document == null || widget == null) return;

            if (document.HiddenWidgets.Contains(widget))
                document.HiddenWidgets.Remove(widget);
            else if (!document.ShownWidgets.Contains(widget))
                document.ShownWidgets.Add(widget);
            SaveDocument(document);

        }

        public TextPage Get404Page()
        {
            var error404Id = _siteSettings.Error404PageId;

            return _session.Get<TextPage>(error404Id)
                   ?? GetDocumentByUrl<TextPage>("404")
                   ?? MrCMSApplication.PublishedRootChildren.OfType<TextPage>().FirstOrDefault();
        }

        public TextPage Get500Page()
        {
            var error500Id = _siteSettings.Error500PageId;

            return _session.Get<TextPage>(error500Id)
                   ?? GetDocumentByUrl<TextPage>("500")
                   ?? MrCMSApplication.PublishedRootChildren.OfType<TextPage>().FirstOrDefault();
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

        public DocumentTypeDefinition GetDefinitionByType(Type type)
        {
            return DocumentTypeHelper.GetDefinitionByType(type);
        }
    }
}