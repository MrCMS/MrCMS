using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using MrCMS.Settings;

namespace MrCMS.Indexing.Querying
{
    public sealed class Searcher<TEntity, TDefinition> : ISearcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        private readonly TDefinition _definition;
        private readonly Site _site;
        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly SiteSettings _siteSettings;
        private bool _disposed;
        private IndexSearcher _indexSearcher;
        private Directory _directory;

        public Searcher(Site site, IGetLuceneDirectory getLuceneDirectory, TDefinition definition, SiteSettings siteSettings)
        {
            _site = site;
            _getLuceneDirectory = getLuceneDirectory;
            _definition = definition;
            _siteSettings = siteSettings;
            IndexManager.EnsureIndexExists<TEntity, TDefinition>();
        }

        public string IndexFolderName
        {
            get { return Definition.IndexFolderName; }
        }

        public TDefinition Definition
        {
            get { return _definition; }
        }

        public IPagedList<TEntity> Search(Query query, int pageNumber, int? pageSize = null, Filter filter = null,
            Sort sort = null)
        {
            int size = pageSize ?? _siteSettings.DefaultPageSize;

            TopFieldDocs topDocs = IndexSearcher.Search(query, filter, pageNumber * size, sort ?? Sort.RELEVANCE);

            IEnumerable<TEntity> entities =
                Definition.Convert(topDocs.ScoreDocs.Skip((pageNumber - 1) * size)
                    .Take(size)
                    .Select(doc => IndexSearcher.Doc(doc.Doc)));

            return new StaticPagedList<TEntity>(entities, pageNumber, size, topDocs.TotalHits);
        }

        public IPagedList<TSubclass> Search<TSubclass>(Query query, int pageNumber, int? pageSize = null,
            Filter filter = null, Sort sort = null) where TSubclass : TEntity
        {
            int size = pageSize ?? _siteSettings.DefaultPageSize;
            BooleanQuery booleanQuery = null;
            if (query is MatchAllDocsQuery)
            {
                booleanQuery = new BooleanQuery();
            }
            else if (query is BooleanQuery)
            {
                booleanQuery = query as BooleanQuery;
            }
            if (booleanQuery != null)
                booleanQuery.Add(
                    new TermQuery(new Term(IndexDefinition<TEntity>.EntityType.FieldName, typeof(TSubclass).FullName)),
                    Occur.MUST);

            TopFieldDocs topDocs = IndexSearcher.Search(booleanQuery ?? query, filter, pageNumber * size,
                sort ?? Sort.RELEVANCE);

            IEnumerable<TSubclass> entities =
                Definition.Convert<TSubclass>(topDocs.ScoreDocs.Skip((pageNumber - 1) * size)
                    .Take(size)
                    .Select(doc => IndexSearcher.Doc(doc.Doc)));

            return new StaticPagedList<TSubclass>(entities, pageNumber, size, topDocs.TotalHits);
        }

        public int Total(Query query, Filter filter = null)
        {
            TopDocs topDocs = IndexSearcher.Search(query, filter, int.MaxValue);

            return topDocs.TotalHits;
        }

        public IList<TEntity> GetAll(Query query = null, Filter filter = null, Sort sort = null)
        {
            TopFieldDocs topDocs = IndexSearcher.Search(query, filter, int.MaxValue, sort ?? Sort.RELEVANCE);

            IEnumerable<TEntity> entities =
                Definition.Convert(topDocs.ScoreDocs.Select(doc => IndexSearcher.Doc(doc.Doc)));

            return entities.ToList();
        }

        public IndexSearcher IndexSearcher
        {
            get { return _indexSearcher = _indexSearcher ?? new IndexSearcher(GetDirectory(_site)); }
        }

        public string IndexName
        {
            get { return Definition.IndexName; }
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        private Directory GetDirectory(Site site)
        {
            return _directory = _directory ?? _getLuceneDirectory.Get(site, IndexFolderName);
        }

        protected void Dispose(bool disposing)
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