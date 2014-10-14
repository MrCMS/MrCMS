using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly Site _currentSite;
        private readonly ISession _session;

        public DocumentService(ISession session, Site currentSite)
        {
            _session = session;
            _currentSite = currentSite;
        }

        public void AddDocument<T>(T document) where T : Document
        {
            _session.Transact(session => session.Save(document));
        }

        public T GetDocument<T>(int id) where T : Document
        {
            return _session.Get<T>(id);
        }

        public T SaveDocument<T>(T document) where T : Document
        {
            _session.Transact(session => session.Update(document));
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
                _session.Transact(session => session.Delete(document));
            }
        }

        public void PublishNow(Webpage document)
        {
            if (document.PublishOn == null)
            {
                document.PublishOn = CurrentRequestData.Now;
                _session.Transact(session => session.Update(document));
            }
        }

        public void Unpublish(Webpage document)
        {
            document.PublishOn = null;
            _session.Transact(session => session.Update(document));
        }

        public IList<Tag>  GetDocumentTags(Document document)
        {
            return _session.Query<Tag>()
                            .Where(x => x.Documents.Contains(document)).ToList();
        }

        public T GetDocumentByUrl<T>(string url) where T : Document                        
        {   
            return _session.QueryOver<T>()
                .Where(doc => doc.UrlSegment == url && doc.Site.Id == _currentSite.Id)
                .Take(1).Cacheable()
                .SingleOrDefault();
        }
    }
}