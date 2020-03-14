using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        TDefinition Definition { get; }

        string IndexName { get; }

        Task<IPagedList<TEntity>> Search(Query query, int pageNumber, int? pageSize = null, Filter filter = null,
            Sort sort = null);

        Task<IPagedList<TSubclass>> Search<TSubclass>(Query query, int pageNumber, int? pageSize = null, Filter filter = null,
            Sort sort = null) where TSubclass : TEntity;

        int Total(Query query, Filter filter = null);
        int Total<TSubclass>(Query query, Filter filter = null) where TSubclass : TEntity;

        Task<IList<TEntity>> GetAll(Query query = null, Filter filter = null, Sort sort = null);
        Task<IList<TSubclass>> GetAll<TSubclass>(Query query = null, Filter filter = null, Sort sort = null)
            where TSubclass : TEntity;
    }
}