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
using System;
using System.Linq;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Search
{
    public class UniversalSearchIndexManager : IUniversalSearchIndexManager
    {
        private const string FolderName = "UniversalSearch";
        private readonly Site _site;
        private readonly IGetLuceneIndexWriter _getLuceneIndexWriter;
        private readonly IGetLuceneIndexSearcher _getLuceneIndexSearcher;
        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly IUniversalSearchItemGenerator _universalSearchItemGenerator;
        protected Analyzer Analyser;

        public UniversalSearchIndexManager(IUniversalSearchItemGenerator universalSearchItemGenerator, Site site,
            IGetLuceneIndexWriter getLuceneIndexWriter, IGetLuceneIndexSearcher getLuceneIndexSearcher, IGetLuceneDirectory getLuceneDirectory)
        {
            _universalSearchItemGenerator = universalSearchItemGenerator;
            _site = site;
            _getLuceneIndexWriter = getLuceneIndexWriter;
            _getLuceneIndexSearcher = getLuceneIndexSearcher;
            _getLuceneDirectory = getLuceneDirectory;
        }

        private bool IndexExists => IndexReader.IndexExists(GetDirectory(_site));

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
            {
                CurrentRequestData.OnEndRequest.Add(new AddUniversalSearchTaskInfo(data));
            }
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
            {
                CurrentRequestData.OnEndRequest.Add(new AddUniversalSearchTaskInfo(data));
            }
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
            {
                CurrentRequestData.OnEndRequest.Add(new AddUniversalSearchTaskInfo(data));
            }
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
            return _getLuceneIndexSearcher.Get(FolderName);
        }


        public void EnsureIndexExists()
        {
            if (!IndexExists)
            {
                ReindexAll();
            }
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
        private static readonly object LockObject = new object();

        public void Write(Action<IndexWriter> writeFunc, bool recreateIndex = false)
        {
            if (recreateIndex)
            {
                RecreateIndex();
            }

            lock (LockObject)
            {
                using (var indexWriter = _getLuceneIndexWriter.Get(FolderName, GetAnalyser()))
                {
                    writeFunc(indexWriter);
                    indexWriter.Commit();
                }
                _getLuceneDirectory.ResetRamDirectory(_site, FolderName);
            }

            _getLuceneIndexSearcher.Reset(FolderName);
        }

        private void RecreateIndex()
        {
            _getLuceneIndexWriter.RecreateIndex(FolderName, GetAnalyser());
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
            {
                return null;
            }

            using (IndexReader indexReader = IndexReader.Open(GetDirectory(_site), true))
            {
                return indexReader.NumDocs();
            }
        }

        private DateTime? GetLastModified()
        {
            try
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
            catch
            {
                return null;
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
            return _getLuceneDirectory.GetStandardDictionary(site, FolderName);
        }
    }
}