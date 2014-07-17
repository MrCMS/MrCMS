using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Events.Documents;
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
        private readonly Site _currentSite;
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public DocumentService(ISession session, SiteSettings siteSettings, Site currentSite)
        {
            _session = session;
            _siteSettings = siteSettings;
            _currentSite = currentSite;
        }

        public void AddDocument<T>(T document) where T : Document
        {
            _session.Transact(session =>
            {
                document.DisplayOrder = GetMaxParentDisplayOrder(document);
                document.CustomInitialization(this, _session);
                session.SaveOrUpdate(document);
            });
            EventContext.Instance.Publish<IOnDocumentAdded, OnDocumentAddedEventArgs>(
                new OnDocumentAddedEventArgs(document));
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
                session.Update(document);
            });
            EventContext.Instance.Publish<IOnDocumentUpdated, OnDocumentUpdatedEventArgs>(
                new OnDocumentUpdatedEventArgs(document, CurrentRequestData.CurrentUser));
            return document;
        }

        public IEnumerable<T> GetAllDocuments<T>() where T : Document
        {
            return _session.QueryOver<T>().Where(arg => arg.Site.Id == _currentSite.Id).Cacheable().List();
        }


        public IEnumerable<T> GetDocumentsByParent<T>(T parent) where T : Document
        {
            IEnumerable<T> list = parent != null
                ? _session.QueryOver<T>()
                    .Where(arg => arg.Parent.Id == parent.Id && arg.Site.Id == _currentSite.Id)
                    .OrderBy(arg => arg.DisplayOrder)
                    .Asc.Cacheable()
                    .List()
                : _session.QueryOver<T>()
                    .Where(arg => arg.Parent == null && arg.Site.Id == _currentSite.Id)
                    .OrderBy(arg => arg.DisplayOrder)
                    .Asc.Cacheable()
                    .List();
            return list;
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

        public void DeleteDocument<T>(T document) where T : Document
        {
            if (document != null)
            {
                _session.Transact(session =>
                {
                    document.OnDeleting(session);
                    session.Delete(document);
                });

                EventContext.Instance.Publish<IOnDocumentDeleted, OnDocumentDeletedEventArgs>(
                    new OnDocumentDeletedEventArgs(document));
            }
        }

        public void PublishNow(Webpage document)
        {
            if (document.PublishOn == null)
            {
                document.PublishOn = CurrentRequestData.Now;
                _session.Transact(session => session.Update(document));
                EventContext.Instance.Publish<IOnWebpagePublished, OnWebpagePublishedEventArgs>(
                    new OnWebpagePublishedEventArgs(document));
            }
        }

        public void Unpublish(Webpage document)
        {
            document.PublishOn = null;
            _session.Transact(session => session.Update(document));
            EventContext.Instance.Publish<IOnWebpageUnpublished, OnWebpageUnpublishedEventArgs>(
                new OnWebpageUnpublishedEventArgs(document));
        }

        public T GetDocumentByUrl<T>(string url) where T : Document
        {
            return _session.QueryOver<T>()
                .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                .Take(1).Cacheable()
                .SingleOrDefault();
        }

        public Layout GetDefaultLayout(Webpage currentPage)
        {
            if (currentPage != null)
            {
                string defaultLayoutName = currentPage.GetMetadata().DefaultLayoutName;
                if (!String.IsNullOrEmpty(defaultLayoutName))
                {
                    Layout layout =
                        _session.QueryOver<Layout>()
                            .Where(x => x.Name == defaultLayoutName)
                            .Cacheable()
                            .List()
                            .FirstOrDefault();
                    if (layout != null)
                        return layout;
                }
            }
            int settingValue = _siteSettings.DefaultLayoutId;

            return _session.Get<Layout>(settingValue) ??
                   _session.QueryOver<Layout>()
                       .Where(layout => layout.Site == currentPage.Site)
                       .Take(1)
                       .Cacheable()
                       .SingleOrDefault();
        }


        private int GetMaxParentDisplayOrder(Document document)
        {
            if (document.Parent != null)
            {
                return _session.QueryOver<Document>()
                    .Where(doc => doc.Parent.Id == document.Parent.Id)
                    .Select(Projections.Max<Document>(d => d.DisplayOrder))
                    .SingleOrDefault<int>();
            }
            if (document is MediaCategory)
            {
                List<MediaCategory> documentsByParent = GetDocumentsByParent<MediaCategory>(null).ToList();
                return documentsByParent.Any()
                    ? documentsByParent.Max(category => category.DisplayOrder) + 1
                    : 0;
            }
            if (document is Layout)
            {
                List<Layout> documentsByParent = GetDocumentsByParent<Layout>(null).ToList();
                return documentsByParent.Any()
                    ? documentsByParent.Max(category => category.DisplayOrder) + 1
                    : 0;
            }
            else
            {
                List<Webpage> documentsByParent = GetDocumentsByParent<Webpage>(null).ToList();
                return documentsByParent.Any()
                    ? documentsByParent.Max(category => category.DisplayOrder) + 1
                    : 0;
            }
        }
    }
}