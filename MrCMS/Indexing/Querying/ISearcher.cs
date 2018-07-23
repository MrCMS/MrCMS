using System;
using System.Collections.Generic;
using Lucene.Net.Search;
using MrCMS.Entities;
using MrCMS.Indexing.Management;
using X.PagedList;

namespace MrCMS.Indexing.Querying
{
    public interface ISearcher<TEntity, TDefinition> : IDisposable
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        IndexSearcher IndexSearcher { get; }

        string IndexName { get; }

        IPagedList<TEntity> Search(Query query, int pageNumber, int? pageSize = null, Filter filter = null,
            Sort sort = null);

        IPagedList<TSubclass> Search<TSubclass>(Query query, int pageNumber, int? pageSize = null, Filter filter = null,
            Sort sort = null) where TSubclass : TEntity;

        int Total(Query query, Filter filter = null);
        int Total<TSubclass>(Query query, Filter filter = null) where TSubclass : TEntity;

        IList<TEntity> GetAll(Query query = null, Filter filter = null, Sort sort = null);

        IList<TSubclass> GetAll<TSubclass>(Query query = null, Filter filter = null, Sort sort = null)
            where TSubclass : TEntity;
    }
}