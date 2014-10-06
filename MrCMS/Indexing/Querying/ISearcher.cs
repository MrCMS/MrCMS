using System;
using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities;
using MrCMS.Indexing.Management;
using MrCMS.Paging;

namespace MrCMS.Indexing.Querying
{
    public interface ISearcher<TEntity, TDefinition> : IDisposable
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        IPagedList<TEntity> Search(Query query, int pageNumber, int? pageSize = null, Filter filter = null, Sort sort = null);

        IPagedList<TSubclass> Search<TSubclass>(Query query, int pageNumber, int? pageSize = null, Filter filter = null,
            Sort sort = null) where TSubclass : TEntity;
        int Total(Query query, Filter filter = null);
        IList<TEntity> GetAll(Query query = null, Filter filter = null, Sort sort = null);
        IndexSearcher IndexSearcher { get; }

        string IndexName { get; }
    }
}