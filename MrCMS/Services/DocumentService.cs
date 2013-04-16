using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
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
            var sameParentDocs = GetDocumentsByParent(document.Parent as T).ToList();
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
                        .SetProjection(Projections.RowCount()).SetCacheable(true)
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
            var queryOver = _session.QueryOver<T>().Where(arg => arg.Site.Id == _currentSite.Id);

            queryOver = parent != null
                            ? queryOver.Where(arg => arg.Parent.Id == parent.Id)
                            : queryOver.Where(arg => arg.Parent == null);

            IEnumerable<T> children =
                queryOver.Cacheable().List();

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
            var queryOver = _session.QueryOver<T>().Where(arg => arg.Site.Id == _currentSite.Id);

            queryOver = parent != null
                            ? queryOver.Where(arg => arg.Parent.Id == parent.Id)
                            : queryOver.Where(arg => arg.Parent == null);

            IEnumerable<T> children =
                queryOver.Cacheable().List();

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

            if (!UrlIsValidForWebpage(stringBuilder.ToString(), null))
            {
                var counter = 1;

                while (!UrlIsValidForWebpage(string.Format("{0}-{1}", stringBuilder, counter), null))
                    counter++;

                stringBuilder.AppendFormat("-{0}", counter);
            }
            return stringBuilder.ToString();
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
            });
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
            return _session.QueryOver<Webpage>().Where(webpage => webpage.PublishOn != null && webpage.PublishOn <= DateTime.Now).Cacheable().RowCount() > 0;
        }

        public bool AnyWebpages()
        {
            return _session.QueryOver<Webpage>().Cacheable().RowCount() > 0;
        }

        public IEnumerable<Webpage> GetWebPagesByParentIdForNav(int parentId)
        {
            return
                _session.QueryOver<Webpage>().Where(
                    x => x.Parent.Id == parentId && x.PublishOn != null && x.PublishOn <= DateTime.Now && x.PublishOn < DateTime.Now && x.RevealInNavigation).
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

        public void SetFrontEndRoles(string frontEndRoles, Webpage webpage)
        {
            if (webpage == null) throw new ArgumentNullException("webpage");

            if (frontEndRoles == null)
                frontEndRoles = string.Empty;

            var roleNames =
                frontEndRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x));

            var roles = webpage.FrontEndAllowedRoles.ToList();

            if (webpage.InheritFrontEndRolesFromParent)
            {
                roles.ForEach(role =>
                {
                    role.FrontEndWebpages.Remove(webpage);
                    webpage.FrontEndAllowedRoles.Remove(role);
                });
            }
            else
            {
                roleNames.ForEach(name =>
                {
                    var role = GetRole(name);
                    if (role != null)
                    {
                        if (!webpage.FrontEndAllowedRoles.Contains(role))
                        {
                            webpage.FrontEndAllowedRoles.Add(role);
                            role.FrontEndWebpages.Add(webpage);
                        }
                        roles.Remove(role);
                    }

                });

                roles.ForEach(role =>
                {
                    webpage.FrontEndAllowedRoles.Remove(role);
                    role.FrontEndWebpages.Remove(webpage);
                });
            }

        }

        private UserRole GetRole(string name)
        {
            return _session.QueryOver<UserRole>().Where(role => role.Name.IsInsensitiveLike(name, MatchMode.Exact)).SingleOrDefault();
        }

        public void SetAdminRoles(string adminRoles, Webpage webpage)
        {
            if (webpage == null) throw new ArgumentNullException("webpage");

            if (adminRoles == null)
                adminRoles = string.Empty;

            var roleNames =
                adminRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(
                    x => !string.IsNullOrWhiteSpace(x));

            var roles = webpage.AdminAllowedRoles.ToList();

            if (webpage.InheritAdminRolesFromParent)
            {
                roles.ForEach(role =>
                {
                    role.AdminWebpages.Remove(webpage);
                    webpage.AdminAllowedRoles.Remove(role);
                });
            }
            else
            {
                roleNames.ForEach(name =>
                {
                    var role = GetRole(name);
                    if (!webpage.AdminAllowedRoles.Contains(role))
                    {
                        webpage.AdminAllowedRoles.Add(role);
                        role.AdminWebpages.Add(webpage);
                    }
                    roles.Remove(role);
                });

                roles.ForEach(role =>
                {
                    webpage.AdminAllowedRoles.Remove(role);
                    role.AdminWebpages.Remove(webpage);
                });
            }
        }

        public T GetDocumentByUrl<T>(string url) where T : Document
        {
            return _session.QueryOver<T>()
                        .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                        .Take(1).Cacheable()
                        .SingleOrDefault();
        }

        public bool UrlIsValidForWebpage(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = GetDocument<Webpage>(id.Value);
                var documentHistory = document.Urls.Any(x => x.UrlSegment == url);
                if (url.Trim() == document.UrlSegment.Trim() || documentHistory) //if url is the same or has been used for the same page before lets go
                    return true;

                return !WebpageExists(url) && !ExistsInUrlHistory(url);
            }

            return !WebpageExists(url) && !ExistsInUrlHistory(url);
        }

        /// <summary>
        /// Check to see if the supplied URL is ok to be added to the URL history table
        /// </summary>
        public bool UrlIsValidForWebpageUrlHistory(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return !WebpageExists(url) && !ExistsInUrlHistory(url);
        }

        public bool UrlIsValidForMediaCategory(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = GetDocument<MediaCategory>(id.Value);
                if (url.Trim() == document.UrlSegment.Trim())
                    return true;
                return !MediaCtegoryExists(url);
            }

            return !MediaCtegoryExists(url);
        }

        public bool UrlIsValidForLayout(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = GetDocument<Layout>(id.Value);
                if (url.Trim() == document.UrlSegment.Trim())
                    return true;
                return !LayoutExists(url);
            }

            return !LayoutExists(url);
        }

        public UrlHistory GetHistoryItemByUrl(string url)
        {
            return _session.QueryOver<UrlHistory>()
                        .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                        .Take(1).Cacheable()
                        .SingleOrDefault();
        }

        /// <summary>
        /// Checks to see if the supplied url is unique for media category / folder.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private bool MediaCtegoryExists(string url)
        {
            return _session.QueryOver<MediaCategory>()
                        .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                        .Cacheable()
                        .RowCount() > 0;
        }

        /// <summary>
        /// Checks to see if the supplied url is unique for layouts
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool LayoutExists(string url)
        {
            return _session.QueryOver<Layout>()
                        .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                        .Cacheable()
                        .RowCount() > 0;
        }

        /// <summary>
        /// Checks to see if a webpage exists with the supplied URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private bool WebpageExists(string url)
        {
            return _session.QueryOver<Webpage>()
                        .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                        .Cacheable()
                        .RowCount() > 0;
        }

        /// <summary>
        /// Checks to see if the supplied URL exists in webpage URL history table
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private bool ExistsInUrlHistory(string url)
        {
            return
                _session.QueryOver<UrlHistory>()
                        .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                        .Cacheable()
                        .RowCount() > 0;
        }
    }
}