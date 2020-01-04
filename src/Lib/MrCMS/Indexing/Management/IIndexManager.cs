using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public interface IIndexManager<TEntity, TDefinition> : IIndexManagerBase
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        IndexResult Insert(IEnumerable<TEntity> entities);
        IndexResult Insert(TEntity entity);
        IndexResult Update(IEnumerable<TEntity> entities);
        IndexResult Update(TEntity entity);
        IndexResult Delete(IEnumerable<TEntity> entities);
        IndexResult Delete(TEntity entity);
    }

    public interface IIndexManagerBase
    {
        bool IndexExists { get; }
        int? NumberOfDocs { get; }
        string IndexName { get; }
        IndexCreationResult CreateIndex();
        Type GetIndexDefinitionType();
        Type GetEntityType();
        void Write(Action<IndexWriter> action);
        Document GetDocument(object entity);
        IndexResult Update(object entity);
        IndexResult Insert(object entity);
        IndexResult Delete(object entity);
        void ResetSearcher();
        Task<IndexResult> ReIndex();
        IndexDefinition Definition { get; }
    }
}