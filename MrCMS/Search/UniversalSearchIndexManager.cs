using System;
using System.Collections.Generic;
using System.Linq;
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
        private static IndexSearcher _searcher;
        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly Site _site;
        private readonly IUniversalSearchItemGenerator _universalSearchItemGenerator;
        protected Analyzer Analyser;
        private Directory _directory;

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

        public void Insert(SystemEntity entity)
        {
            if (!_universalSearchItemGenerator.CanGenerate(entity))
            {
                return;
            }

            var data = new UniversalSearchIndexData
            {
                Action = UniversalSearchIndexAction.Insert,
                UniversalSearchItem = _universalSearchItemGenerator.GenerateItem(entity)
            };

            if (!AnyExistInEndRequest(data))
                CurrentRequestData.OnEndRequest.Add(new AddUniversalSearchTaskInfo(data));
        }

        public void Update(SystemEntity entity)
        {
            if (!_universalSearchItemGenerator.CanGenerate(entity))
            {
                return;
            }
            var data = new UniversalSearchIndexData
            {
                Action = UniversalSearchIndexAction.Update,
                UniversalSearchItem = _universalSearchItemGenerator.GenerateItem(entity)
            };

            if (!AnyExistInEndRequest(data))
                CurrentRequestData.OnEndRequest.Add(new AddUniversalSearchTaskInfo(data));
        }

        public void Delete(SystemEntity entity)
        {
            if (!_universalSearchItemGenerator.CanGenerate(entity))
            {
                return;
            }

            var data = new UniversalSearchIndexData
            {
                Action = UniversalSearchIndexAction.Delete,
                UniversalSearchItem = _universalSearchItemGenerator.GenerateItem(entity)
            };

            if (!AnyExistInEndRequest(data))
                CurrentRequestData.OnEndRequest.Add(new AddUniversalSearchTaskInfo(data));
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
            EnsureIndexExists();
            return _searcher ?? (_searcher = new IndexSearcher(GetDirectory(_site), true));
        }


        public void EnsureIndexExists()
        {
            if (!IndexExists)
                ReindexAll();
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

        public void Write(Action<IndexWriter> writeFunc, bool recreateIndex = false)
        {
            if (recreateIndex)
                RecreateIndex();
            var indexWriter = GetIndexWriter();
            writeFunc(indexWriter);
            indexWriter.Commit();
            _searcher = null;
        }

        private void RecreateIndex()
        {
            if (Writers.ContainsKey(_site.Id))
            {
                var existing = Writers[_site.Id];
                if (existing != null) existing.Dispose();
                Writers.Remove(_site.Id);
            }
            using (GetNewIndexWriter(true)) { }
        }

        private static readonly Dictionary<int, IndexWriter> Writers = new Dictionary<int, IndexWriter>();
        private static readonly object LockObject = new object();
        private IndexWriter GetIndexWriter()
        {
            lock (LockObject)
            {
                if (!Writers.ContainsKey(_site.Id))
                {
                    Writers[_site.Id] = GetNewIndexWriter(false);
                }
                return Writers[_site.Id];
            }
        }

        private IndexWriter GetNewIndexWriter(bool recreateIndex)
        {
            return new IndexWriter(GetDirectory(_site), GetAnalyser(), recreateIndex,
                IndexWriter.MaxFieldLength.UNLIMITED);
        }

        private static bool AnyExistInEndRequest(UniversalSearchIndexData data)
        {
            return
                CurrentRequestData.OnEndRequest.OfType<AddUniversalSearchTaskInfo>()
                    .Any(task => UniversalSearchIndexData.Comparer.Equals(data, task.Data));
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
            DateTime time;
            var sourceTimeZone = TimeZoneInfo.Utc;
            try
            {
                time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(lastModified);
            }
            catch 
            {
                time = DateTime.FromFileTime(lastModified);
                sourceTimeZone = TimeZoneInfo.Local;
            }

            return TimeZoneInfo.ConvertTime(time, sourceTimeZone, CurrentRequestData.TimeZoneInfo);
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
    }
}