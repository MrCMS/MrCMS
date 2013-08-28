using System;
using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using System.Linq;

namespace MrCMS.Indexing.Management
{
    public abstract class IndexManager<TEntity, TDefinition> : IIndexManager<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IIndexDefinition<TEntity>, new()
    {
        protected readonly Site CurrentSite;

        protected IndexManager(Site currentSite)
        {
            CurrentSite = currentSite;
        }

        protected readonly TDefinition Definition = new TDefinition();

        protected abstract Directory GetDirectory();

        public bool IndexExists
        {
            get { return IndexReader.IndexExists(GetDirectory()); }
        }

        public DateTime? LastModified
        {
            get
            {
                if (!IndexExists)
                    return null;

                var lastModified = IndexReader.LastModified(GetDirectory());

                return new DateTime(1970, 1, 1).AddMilliseconds(lastModified);
            }
        }

        public int? NumberOfDocs
        {
            get
            {
                if (!IndexExists)
                    return null;

                return IndexReader.Open(GetDirectory(), true).NumDocs();
            }
        }

        public string IndexName { get { return Definition.IndexName; } }

        private void Write(Action<IndexWriter> writeFunc, bool recreateIndex = false)
        {
            using (
                var indexWriter = new IndexWriter(GetDirectory(), Definition.GetAnalyser(), recreateIndex,
                                                  IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writeFunc(indexWriter);
            }
        }

        public IndexCreationResult CreateIndex()
        {
            var fsDirectory = GetDirectory();
            var indexExists = IndexReader.IndexExists(fsDirectory);
            if (indexExists)
                return IndexCreationResult.AlreadyExists;
            try
            {
                Write(writer => { }, true);
                return IndexCreationResult.Success;
            }
            catch
            {
                return IndexCreationResult.Failure;
            }
        }

        public Type GetIndexDefinitionType()
        {
            return typeof(TDefinition);
        }

        public Type GetEntityType()
        {
            return typeof(TEntity);
        }

        public IndexResult Insert(IEnumerable<TEntity> entities)
        {
            return IndexResult.GetResult(() => Write(writer =>
                                                         {
                                                             foreach (var entity in entities)
                                                                 writer.AddDocument(Definition.Convert(entity));
                                                         }));
        }

        public IndexResult Insert(TEntity entity)
        {
            return IndexResult.GetResult(() => Write(writer => writer.AddDocument(Definition.Convert(entity))));
        }

        public IndexResult Insert(object entity)
        {
            if (entity is TEntity)
                return Insert(entity as TEntity);

            return IndexResult.GetResult(() =>
            {
                throw new Exception(
                    string.Format(
                        "object {0} is not of correct type for the index {1}",
                        entity,
                        GetType().Name));
            });
        }

        public IndexResult Delete(object entity)
        {
            if (entity is TEntity)
                return Delete(entity as TEntity);

            return IndexResult.GetResult(() =>
            {
                throw new Exception(
                    string.Format(
                        "object {0} is not of correct type for the index {1}",
                        entity.ToString(),
                        GetType().Name));
            });
        }

        public IndexResult Update(IEnumerable<TEntity> entities)
        {
            return IndexResult.GetResult(() => Write(writer =>
                                                         {
                                                             foreach (var entity in entities)
                                                                 writer.UpdateDocument(Definition.GetIndex(entity),
                                                                                       Definition.Convert(entity));
                                                         }));
        }

        public IndexResult Update(TEntity entity)
        {
            return IndexResult.GetResult(() => Write(writer =>
                                                         {
                                                             using (var indexSearcher = new IndexSearcher(GetDirectory(), true))
                                                             {
                                                                 var topDocs = indexSearcher.Search(new TermQuery(Definition.GetIndex(entity)), int.MaxValue);
                                                                 if (!topDocs.ScoreDocs.Any()) 
                                                                    return;
                                                             }
                                                             writer.UpdateDocument(Definition.GetIndex(entity),
                                                                                   Definition.Convert(entity));
                                                         }));
        }

        public IndexResult Update(object entity)
        {
            if (entity is TEntity)
                return Update(entity as TEntity);

            return IndexResult.GetResult(() =>
                                             {
                                                 throw new Exception(
                                                     string.Format(
                                                         "object {0} is not of correct type for the index {1}", entity, GetType().Name));
                                             });
        }

        public IndexResult Delete(IEnumerable<TEntity> entities)
        {
            return IndexResult.GetResult(() => Write(writer =>
                                                         {
                                                             foreach (var entity in entities)
                                                                 writer.DeleteDocuments(Definition.GetIndex(entity));
                                                         }));
        }

        public IndexResult Delete(TEntity entity)
        {
            return IndexResult.GetResult(() => Write(writer => writer.DeleteDocuments(Definition.GetIndex(entity))));
        }

        public IndexResult Optimise()
        {
            return IndexResult.GetResult(() => Write(writer => writer.Optimize()));
        }

        public IndexResult ReIndex(IEnumerable<TEntity> entities)
        {
            return IndexResult.GetResult(() =>
                                             {
                                                 Write(writer => { }, true);
                                                 Write(writer =>
                                                           {
                                                               foreach (var entity in entities)
                                                                   writer.AddDocument(Definition.Convert(entity));
                                                               writer.Optimize();
                                                           });
                                             });
        }
    }
}