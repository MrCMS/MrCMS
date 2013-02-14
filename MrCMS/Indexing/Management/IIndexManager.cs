using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System.Linq;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Management
{
    public interface IIndexManager<in TEntity, TDefinition>
        where TEntity : class
        where TDefinition : IIndexDefinition<TEntity>, new()
    {
        bool DoesIndexExist();
        IndexCreationResult CreateIndex();

        IndexResult Insert(IEnumerable<TEntity> entities);
        IndexResult Insert(TEntity entity);
        IndexResult Update(IEnumerable<TEntity> entities);
        IndexResult Update(TEntity entity);
        IndexResult Delete(IEnumerable<TEntity> entities);
        IndexResult Delete(TEntity entity);

        IndexResult Optimise();
    }

    public abstract class IndexManager<TEntity, TDefinition> : IIndexManager<TEntity, TDefinition>
        where TEntity : class
        where TDefinition : IIndexDefinition<TEntity>, new()
    {
        protected readonly TDefinition Definition = new TDefinition();

        protected abstract Directory GetDirectory();

        public bool DoesIndexExist()
        {
            return IndexReader.IndexExists(GetDirectory());
        }

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
            return IndexResult.GetResult(() => Write(writer => writer.UpdateDocument(Definition.GetIndex(entity),
                                                                                     Definition.Convert(entity))));
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
    }

    public class FSDirectoryIndexManager<TEntity, TDefinition> : IndexManager<TEntity, TDefinition>
        where TEntity : class
        where TDefinition : IIndexDefinition<TEntity>, new()
    {
        protected override Directory GetDirectory()
        {
            return FSDirectory.Open(new DirectoryInfo(Definition.GetLocation()));
        }
    }
}