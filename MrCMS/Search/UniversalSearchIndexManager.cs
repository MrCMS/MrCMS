using System;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using MrCMS.Website;
using StackExchange.Profiling;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Search
{
    public class UniversalSearchIndexManager : IUniversalSearchIndexManager
    {
        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly Site _site;
        private readonly IUniversalSearchItemGenerator _universalSearchItemGenerator;
        protected Analyzer Analyser;
        private Directory _directory;
        private static IndexSearcher _searcher;

        public UniversalSearchIndexManager(IUniversalSearchItemGenerator universalSearchItemGenerator, Site site,
            IGetLuceneDirectory getLuceneDirectory)
        {
            _universalSearchItemGenerator = universalSearchItemGenerator;
            _site = site;
            _getLuceneDirectory = getLuceneDirectory;
        }

        private bool IndexExists
        {
            get { return IndexReader.IndexExists(GetDirectory(_site)); }
        }

        public void Index(SystemEntity entity)
        {
            if (!_universalSearchItemGenerator.CanGenerate(entity))
            {
                return;
            }
            CurrentRequestData.OnEndRequest.Add(new UpdateUniversalIndex(entity));
        }

        public void Delete(SystemEntity entity)
        {
            if (!_universalSearchItemGenerator.CanGenerate(entity))
            {
                return;
            }
            CurrentRequestData.OnEndRequest.Add(new DeleteFromUniversalIndex(entity));
        }

        public void ReindexAll()
        {
            InitializeIndex();
            Write(writer =>
            {
                using (MiniProfiler.Current.Step("Reindexing"))
                {
                    foreach (Document document in _universalSearchItemGenerator.GetAllItems())
                    {
                        writer.AddDocument(document);
                    }
                }
            });
        }

        public void Optimise()
        {
            Write(writer => writer.Optimize());
        }

        public IndexSearcher GetSearcher()
        {
            return _searcher ?? (_searcher = new IndexSearcher(GetDirectory(_site), true));
        }


        public void EnsureIndexExists()
        {
            if(!IndexExists)
                InitializeIndex();
        }

        public MrCMSIndex GetUniversalIndexInfo()
        {
            return new MrCMSIndex
            {
                DoesIndexExist = IndexExists,
                LastModified = GetLastModified(),
                Name = "Universal Search Index",
                NumberOfDocs = GetNumberOfDocs(),
                TypeName = GetType().FullName
            };
        }

        private int? GetNumberOfDocs()
        {
            if (!IndexExists)
                return null;

            using (IndexReader indexReader = IndexReader.Open(GetDirectory(_site), true))
            {
                return indexReader.NumDocs();
            }
        }

        private DateTime? GetLastModified()
        {
            long lastModified = IndexReader.LastModified(GetDirectory(_site));
            try
            {
                return new DateTime(1970, 1, 1).AddMilliseconds(lastModified);
            }
            catch
            {
                return DateTime.FromFileTime(lastModified);
            }
        }

        private void InitializeIndex()
        {
            Write(writer => { }, true);
        }

        public virtual Analyzer GetAnalyser()
        {
            return Analyser ?? (Analyser = new StandardAnalyzer(Version.LUCENE_30));
        }

        private Directory GetDirectory(Site site)
        {
            return _getLuceneDirectory.Get(site, "UniversalSearch");
        }


        public void Write(Action<IndexWriter> writeFunc, bool recreateIndex = false)
        {
            using (var indexWriter = new IndexWriter(GetDirectory(_site), GetAnalyser(), recreateIndex,
                IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writeFunc(indexWriter);
            }
            _searcher = null;
        }
    }
}