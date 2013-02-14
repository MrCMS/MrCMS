using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace MrCMS.Indexing.Management
{
    public interface IIndexWriterFactory
    {
    }

    public class DirectoryIndexWriterFactory : IIndexWriterFactory
    {
        public bool DoesIndexExist<T>(IIndexDefinition<T> indexDefinition) where T : class
        {
            var fsDirectory = FSDirectory.Open(new DirectoryInfo(indexDefinition.GetLocation()));

            return IndexReader.IndexExists(fsDirectory);
        }

        public IndexCreationResult CreateIndex<T>(IIndexDefinition<T> indexDefinition) where T : class
        {
            var fsDirectory = FSDirectory.Open(new DirectoryInfo(indexDefinition.GetLocation()));

            var indexExists = IndexReader.IndexExists(fsDirectory);
            if (indexExists)
                return IndexCreationResult.AlreadyExists; ;
            try
            {
                using (new IndexWriter(fsDirectory,
                                       indexDefinition.GetAnalyser(),
                                       true,
                                       IndexWriter.MaxFieldLength.UNLIMITED))
                {

                }
                return IndexCreationResult.Success;
            }
            catch
            {
                return IndexCreationResult.Failure;
            }

        }

        public IndexWriter CreateWriter<T>(IIndexDefinition<T> indexDefinition) where T : class
        {
            var fsDirectory = FSDirectory.Open(new DirectoryInfo(indexDefinition.GetLocation()));

            var recreateIndex = !(IndexReader.IndexExists(fsDirectory));

            return new IndexWriter(fsDirectory,
                                        indexDefinition.GetAnalyser(),
                                        recreateIndex,
                                        IndexWriter.MaxFieldLength.UNLIMITED);
        }
    }

    public enum IndexCreationResult
    {
        Success, Failure, AlreadyExists
    }

    /// <summary>
    /// Used as a definition for resolving indexwriters
    /// </summary>
    public class IndexOptions : IEquatable<IndexOptions>
    {
        public IndexOptions()
        {
            this.Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            this.Attributes = new Dictionary<string, object>();
        }

        public bool OptimizeIndex { get; set; }
        public bool RecreateIndex { get; set; }
        public IIndexLocation IndexLocation { get; set; }
        public Analyzer Analyzer { get; set; }
        public IDictionary<string, object> Attributes { get; set; }

        public bool Equals(IndexOptions other)
        {
            if (other == null)
                return false;

            return (AreEqual(other.OptimizeIndex, this.OptimizeIndex))
                && (AreEqual(other.RecreateIndex, this.RecreateIndex))
                && (AreEqual(other.IndexLocation, this.IndexLocation))
                && (AreEqual(other.Analyzer.GetType(), this.Analyzer.GetType()))
                && (CompareAttributes(other.Attributes));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IndexOptions);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected bool AreEqual(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
                return true;
            if (obj1 == null || obj2 == null)
                return false;
            return (obj1.Equals(obj2));
        }

        protected bool CompareAttributes(IDictionary<string, object> other)
        {
            foreach (var item in this.Attributes)
            {
                object obj;
                if (other.TryGetValue(item.Key, out obj))
                {
                    if (!AreEqual(obj, item.Value))
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
    public interface IIndexLocation : IEquatable<IIndexLocation>
    {
        string GetPath();
    }
}