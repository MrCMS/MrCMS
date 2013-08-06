using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using NHibernate;

namespace MrCMS.Indexing.Querying
{
    public abstract class Searcher<TEntity, TDefinition> : ISearcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IIndexDefinition<TEntity>, new()
    {
        private readonly ISession _session;
        protected readonly TDefinition Definition = new TDefinition();
        private IndexSearcher _indexSearcher;

        protected Searcher(Site currentSite, ISession session)
        {
            _session = session;
            _indexSearcher = new IndexSearcher(GetDirectory(currentSite));
        }

        protected abstract Directory GetDirectory(Site currentSite);

        public IPagedList<TEntity> Search(Query query, int pageNumber, int pageSize, Filter filter = null, Sort sort = null)
        {
            var topDocs = IndexSearcher.Search(query, filter, pageNumber * pageSize, sort ?? Sort.RELEVANCE);

            var entities =
                Definition.Convert(_session,
                                   topDocs.ScoreDocs.Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .Select(doc => IndexSearcher.Doc(doc.Doc)));

            return new StaticPagedList<TEntity>(entities, pageNumber, pageSize, topDocs.TotalHits);
        }

        public int Total(Query query, Filter filter = null)
        {
            var topDocs = IndexSearcher.Search(query, filter, int.MaxValue);

            return topDocs.TotalHits;
        }
        
        public IList<TEntity> GetAll(Query query = null, Filter filter = null, Sort sort = null)
        {
            var topDocs = IndexSearcher.Search(query, filter, int.MaxValue, sort ?? Sort.RELEVANCE);

            var entities = Definition.Convert(_session, topDocs.ScoreDocs.Select(doc => IndexSearcher.Doc(doc.Doc)));

            return entities.ToList();
        }

        public IndexSearcher IndexSearcher { get { return _indexSearcher; } }


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