using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Indexing.Querying
{
    public abstract class Searcher<TEntity, TDefinition> : ISearcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        private readonly Site _site;
        private readonly ISession _session;
        private readonly TDefinition _definition;
        private readonly SiteSettings _siteSettings;
        private IndexSearcher _indexSearcher;

        protected Searcher(Site site, ISession session, TDefinition definition, SiteSettings siteSettings)
        {
            _site = site;
            _session = session;
            _definition = definition;
            _siteSettings = siteSettings;
        }

        protected abstract Directory GetDirectory(Site site);

        public IPagedList<TEntity> Search(Query query, int pageNumber, int? pageSize = null, Filter filter = null, Sort sort = null)
        {
            var size = pageSize ?? _siteSettings.DefaultPageSize;

            var topDocs = IndexSearcher.Search(query, filter, pageNumber * size, sort ?? Sort.RELEVANCE);

            var entities =
                Definition.Convert(topDocs.ScoreDocs.Skip((pageNumber - 1) * size)
                                          .Take(size)
                                          .Select(doc => IndexSearcher.Doc(doc.Doc)));

            return new StaticPagedList<TEntity>(entities, pageNumber, size, topDocs.TotalHits);
        }

        public int Total(Query query, Filter filter = null)
        {
            var topDocs = IndexSearcher.Search(query, filter, int.MaxValue);

            return topDocs.TotalHits;
        }
        
        public IList<TEntity> GetAll(Query query = null, Filter filter = null, Sort sort = null)
        {
            var topDocs = IndexSearcher.Search(query, filter, int.MaxValue, sort ?? Sort.RELEVANCE);

            var entities = Definition.Convert(topDocs.ScoreDocs.Select(doc => IndexSearcher.Doc(doc.Doc)));

            return entities.ToList();
        }

        public IndexSearcher IndexSearcher { get { return _indexSearcher = _indexSearcher ?? new IndexSearcher(GetDirectory(_site)); } }

        public string IndexName { get { return Definition.IndexName; } }
        public string IndexFolderName { get { return Definition.IndexFolderName; } }

        public TDefinition Definition
        {
            get { return _definition; }
        }


        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (!_disposed)
            {
                if (disposing)
                {
                    if (IndexSearcher != null)
                        IndexSearcher.Dispose();
                }

                // Indicate that the instance has been disposed.
                _indexSearcher = null;
                _disposed = true;
            }
        }
    }
}