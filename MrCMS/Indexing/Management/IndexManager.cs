using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Ajax.Utilities;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Indexing.Management
{
    public static class IndexManager
    {
        public static void EnsureIndexExists<T1, T2>()
            where T1 : SystemEntity
            where T2 : IndexDefinition<T1>
        {
            var service = MrCMSApplication.Get<IIndexService>();
            IIndexManagerBase indexManagerBase = service.GetIndexManagerBase(typeof(T2));
            if (!indexManagerBase.IndexExists)
            {
                service.Reindex(indexManagerBase.GetIndexDefinitionType().FullName);
            }
        }
    }

    public class IndexManager<TEntity, TDefinition> : IIndexManager<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        private readonly IGetLuceneIndexWriter _getLuceneIndexWriter;
        private readonly IGetLuceneIndexSearcher _getLuceneIndexSearcher;
        private readonly Site _site;
        private readonly TDefinition _definition;
        private readonly IStatelessSession _statelessSession;

        public IndexManager(IGetLuceneIndexWriter getLuceneIndexWriter, IGetLuceneIndexSearcher getLuceneIndexSearcher,
            Site site, TDefinition definition, IStatelessSession statelessSession)
        {
            _getLuceneIndexWriter = getLuceneIndexWriter;
            _getLuceneIndexSearcher = getLuceneIndexSearcher;
            _site = site;
            _definition = definition;
            _statelessSession = statelessSession;
        }

        public string IndexFolderName
        {
            get { return Definition.IndexFolderName; }
        }

        public TDefinition Definition
        {
            get { return _definition; }
        }

        public bool IndexExists
        {
            get { return IndexReader.IndexExists(GetDirectory(_site)); }
        }

        public DateTime? LastModified
        {
            get
            {
                if (!IndexExists)
                    return null;

                long lastModified = IndexReader.LastModified(GetDirectory(_site));
                DateTime time;
                var sourceTimeZone = TimeZoneInfo.Utc;
                try
                {
                    time = new DateTime(1970, 1, 1).AddMilliseconds(lastModified);
                }
                catch
                {
                    time = DateTime.FromFileTime(lastModified);
                    sourceTimeZone = TimeZoneInfo.Local;
                }

                return TimeZoneInfo.ConvertTime(time, sourceTimeZone, CurrentRequestData.TimeZoneInfo);
            }
        }

        public int? NumberOfDocs
        {
            get
            {
                if (!IndexExists)
                    return null;

                using (var indexReader = IndexReader.Open(GetDirectory(_site), true))
                {
                    return indexReader.NumDocs();
                }
            }
        }

        public string IndexName
        {
            get { return Definition.IndexName; }
        }

        public IndexCreationResult CreateIndex()
        {
            Directory fsDirectory = GetDirectory(_site);
            bool indexExists = IndexReader.IndexExists(fsDirectory);
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
                foreach (TEntity entity in entities)
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
                        entity,
                        GetType().Name));
            });
        }

        public void ResetSearcher()
        {
            _getLuceneIndexSearcher.Reset(Definition);
        }

        public IndexResult Update(IEnumerable<TEntity> entities)
        {
            return IndexResult.GetResult(() => Write(writer =>
            {
                foreach (TEntity entity in entities)
                    writer.UpdateDocument(Definition.GetIndex(entity),
                        Definition.Convert(entity));
            }));
        }

        public IndexResult Update(TEntity entity)
        {
            return IndexResult.GetResult(() => Write(writer =>
            {
                var indexSearcher = _getLuceneIndexSearcher.Get(Definition);

                TopDocs topDocs = indexSearcher.Search(new TermQuery(Definition.GetIndex(entity)), int.MaxValue);
                if (!topDocs.ScoreDocs.Any())
                    return;

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
                foreach (TEntity entity in entities)
                    writer.DeleteDocuments(Definition.GetIndex(entity));
            }));
        }

        public IndexResult Delete(TEntity entity)
        {
            return IndexResult.GetResult(() => Write(writer => writer.DeleteDocuments(Definition.GetIndex(entity))));
        }

        public IndexResult ReIndex()
        {
            var criteria = _statelessSession.CreateCriteria(typeof(TEntity))
                .Add(Restrictions.Eq("IsDeleted", false));
            if (typeof(SiteEntity).IsAssignableFrom(typeof(TEntity)))
                criteria.Add(Restrictions.Eq("Site.Id", _site.Id));
            var entities = criteria.SetCacheable(true)
                .List<TEntity>().ToList();
            return IndexResult.GetResult(() =>
            {
                Write(writer =>
                {
                    foreach (Document document in Definition.ConvertAll(entities))
                        writer.AddDocument(document);
                    writer.Optimize();
                }, true);
            });
        }

        public Document GetDocument(object entity)
        {
            return Definition.Convert(entity as TEntity);
        }

        public IndexResult Optimise()
        {
            return IndexResult.GetResult(() => Write(writer => writer.Optimize()));
        }

        private Directory GetDirectory(Site site)
        {
            return _getLuceneIndexWriter.Get(Definition).Directory;
        }

        private void Write(Action<IndexWriter> writeFunc, bool recreateIndex)
        {
            if (recreateIndex)
                _getLuceneIndexWriter.RecreateIndex(Definition);
            var indexWriter = _getLuceneIndexWriter.Get(Definition);
            writeFunc(indexWriter);
            indexWriter.Commit();
            _getLuceneIndexSearcher.Reset(Definition);
        }
    }
}