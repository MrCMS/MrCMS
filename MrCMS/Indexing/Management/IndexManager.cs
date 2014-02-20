using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using System.Linq;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Indexing.Management
{
    public static class IndexManager
    {
        public static void EnsureIndexesExist(IKernel kernel, ISession session, Site site)
        {
            var service = new IndexService(kernel, session, site);
            var mrCMSIndices = service.GetIndexes();
            foreach (var index in mrCMSIndices.Where(index => !index.DoesIndexExist))
            {
                service.Reindex(index.TypeName);
                service.Optimise(index.TypeName);
            }
        }
        public static void EnsureIndexExists<T1,T2>() where T1 :SystemEntity  where T2 : IndexDefinition<T1>
        {
            var service = MrCMSApplication.Get<IIndexService>();
            var indexManagerBase = service.GetIndexManagerBase(typeof (T2));
            if (!indexManagerBase.IndexExists)
            {
                service.Reindex(indexManagerBase.GetIndexDefinitionType().FullName);
            }
        }
    }

    public abstract class IndexManager<TEntity, TDefinition> : IIndexManager<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        private readonly Site _currentSite;
        private readonly TDefinition _definition;

        protected IndexManager(Site currentSite, TDefinition definition)
        {
            _currentSite = currentSite;
            _definition = definition;
        }

        protected abstract Directory GetDirectory(Site site);

        public bool IndexExists
        {
            get { return IndexReader.IndexExists(GetDirectory(_currentSite)); }
        }

        public DateTime? LastModified
        {
            get
            {
                if (!IndexExists)
                    return null;

                var lastModified = IndexReader.LastModified(GetDirectory(_currentSite));
                try
                {
                    return new DateTime(1970, 1, 1).AddMilliseconds(lastModified);
                }
                catch
                {
                    return DateTime.FromFileTime(lastModified);
                }
            }
        }

        public int? NumberOfDocs
        {
            get
            {
                if (!IndexExists)
                    return null;

                return IndexReader.Open(GetDirectory(_currentSite), true).NumDocs();
            }
        }

        public string IndexName { get { return Definition.IndexName; } }
        public string IndexFolderName { get { return Definition.IndexFolderName; } }

        public TDefinition Definition
        {
            get { return _definition; }
        }

        private void Write(Action<IndexWriter> writeFunc, bool recreateIndex)
        {
            using (
                var indexWriter = new IndexWriter(GetDirectory(_currentSite), Definition.GetAnalyser(), recreateIndex,
                                                  IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writeFunc(indexWriter);
            }
        }

        public IndexCreationResult CreateIndex()
        {
            var fsDirectory = GetDirectory(_currentSite);
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

        public void Write(Action<IndexWriter> action)
        {
            Write(action, false);
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
                                                             using (var indexSearcher = new IndexSearcher(GetDirectory(_currentSite), true))
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

        public Document GetDocument(object entity)
        {
            return Definition.Convert(entity as TEntity);
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