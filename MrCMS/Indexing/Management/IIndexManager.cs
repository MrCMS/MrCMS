using System;
using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Indexing.Management
{
    public interface IIndexManager<in TEntity, TDefinition> : IIndexManagerBase
        where TEntity : SystemEntity where TDefinition : IIndexDefinition<TEntity>, new()
    {
        IndexResult Insert(IEnumerable<TEntity> entities);
        IndexResult Insert(TEntity entity);
        IndexResult Update(IEnumerable<TEntity> entities);
        IndexResult Update(TEntity entity);
        IndexResult Delete(IEnumerable<TEntity> entities);
        IndexResult Delete(TEntity entity);

        IndexResult ReIndex(IEnumerable<TEntity> entities);
    }

    public interface IIndexManagerBase
    {
        bool IndexExists { get; }
        DateTime? LastModified { get; }
        int? NumberOfDocs { get; }
        string IndexName { get; }
        IndexCreationResult CreateIndex();
        Type GetIndexDefinitionType();
        Type GetEntityType();
        IndexResult Optimise();
        IndexResult Update(object entity);
        IndexResult Insert(object entity);
        IndexResult Delete(object entity);
    }
}