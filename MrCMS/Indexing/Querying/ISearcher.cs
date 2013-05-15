using System;
using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Paging;

namespace MrCMS.Indexing.Querying
{
    public interface ISearcher<TEntity, TDefinition> : IDisposable
        where TEntity : class
        where TDefinition : IIndexDefinition<TEntity>, new()
    {
        IPagedList<TEntity> Search(Query query, int pageNumber, int pageSize, Filter filter = null);
        int Total(Query query, Filter filter = null);
        IList<TEntity> GetAll(Query query = null, Filter filter = null);
    }
}