using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities;
using MrCMS.Indexing.Management;
using MrCMS.Settings;
using X.PagedList;

namespace MrCMS.Indexing.Querying
{
    public sealed class Searcher<TEntity, TDefinition> : ISearcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        private readonly IGetLuceneIndexSearcher _getLuceneIndexSearcher;
        private readonly SiteSettings _siteSettings;
        private bool _disposed;

        public Searcher(TDefinition definition, IGetLuceneIndexSearcher getLuceneIndexSearcher,
            SiteSettings siteSettings, IIndexManager<TEntity,TDefinition> manager)
        {
            Definition = definition;
            _getLuceneIndexSearcher = getLuceneIndexSearcher;
            _siteSettings = siteSettings;
            if (!manager.IndexExists)
                manager.CreateIndex();
        }

        public TDefinition Definition { get; }

        public IPagedList<TEntity> Search(Query query, int pageNumber, int? pageSize = null, Filter filter = null,
            Sort sort = null)
        {
            var size = pageSize ?? _siteSettings.DefaultPageSize;

            var topDocs = IndexSearcher.Search(query, filter, pageNumber * size, sort ?? Sort.RELEVANCE);

            var entities =
                Definition.Convert(topDocs.ScoreDocs.Skip((pageNumber - 1) * size)
                    .Take(size)
                    .Select(doc => IndexSearcher.Doc(doc.Doc)));

            return new StaticPagedList<TEntity>(entities, pageNumber, size, topDocs.TotalHits);
        }

        public IPagedList<TSubclass> Search<TSubclass>(Query query, int pageNumber, int? pageSize = null,
            Filter filter = null, Sort sort = null) where TSubclass : TEntity
        {
            var size = pageSize ?? _siteSettings.DefaultPageSize;
            var booleanQuery = UpdateQuery<TSubclass>(query);

            var topDocs = IndexSearcher.Search(booleanQuery ?? query, filter, pageNumber * size,
                sort ?? Sort.RELEVANCE);

            var entities =
                Definition.Convert<TSubclass>(topDocs.ScoreDocs.Skip((pageNumber - 1) * size)
                    .Take(size)
                    .Select(doc => IndexSearcher.Doc(doc.Doc)));

            return new StaticPagedList<TSubclass>(entities, pageNumber, size, topDocs.TotalHits);
        }

        public int Total(Query query, Filter filter = null)
        {
            var topDocs = IndexSearcher.Search(query, filter, int.MaxValue);

            return topDocs.TotalHits;
        }

        public int Total<TSubclass>(Query query, Filter filter = null) where TSubclass : TEntity
        {
            var booleanQuery = UpdateQuery<TSubclass>(query);

            var topDocs = IndexSearcher.Search(booleanQuery, filter, int.MaxValue);

            return topDocs.TotalHits;
        }

        public IList<TEntity> GetAll(Query query = null, Filter filter = null, Sort sort = null)
        {
            var topDocs = IndexSearcher.Search(query ?? new MatchAllDocsQuery(), filter, int.MaxValue, sort ?? Sort.RELEVANCE);

            var entities =
                Definition.Convert(topDocs.ScoreDocs.Select(doc => IndexSearcher.Doc(doc.Doc)));

            return entities.ToList();
        }

        public IList<TSubclass> GetAll<TSubclass>(Query query = null, Filter filter = null, Sort sort = null)
            where TSubclass : TEntity
        {
            var booleanQuery = UpdateQuery<TSubclass>(query ?? new MatchAllDocsQuery());

            var topDocs = IndexSearcher.Search(booleanQuery, filter, int.MaxValue, sort ?? Sort.RELEVANCE);

            var entities =
                Definition.Convert<TSubclass>(topDocs.ScoreDocs.Select(doc => IndexSearcher.Doc(doc.Doc)));

            return entities.ToList();
        }

        public IndexSearcher IndexSearcher => _getLuceneIndexSearcher.Get(Definition);

        public string IndexName => Definition.IndexName;

        public void Dispose()
        {
            Dispose(true);

            // Use SuppressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        private static BooleanQuery UpdateQuery<TSubclass>(Query query) where TSubclass : TEntity
        {
            BooleanQuery booleanQuery = null;
            if (query is MatchAllDocsQuery)
                booleanQuery = new BooleanQuery();
            else if (query is BooleanQuery) booleanQuery = query as BooleanQuery;
            if (booleanQuery != null)
                booleanQuery.Add(
                    new TermQuery(new Term(IndexDefinition<TEntity>.EntityType.FieldName, typeof(TSubclass).FullName)),
                    Occur.MUST);
            return booleanQuery;
        }

        private void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (!_disposed)
            {
                if (disposing)
                {
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}