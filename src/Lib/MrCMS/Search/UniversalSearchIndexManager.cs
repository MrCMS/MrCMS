using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Models;
using MrCMS.Website;
using StackExchange.Profiling;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Search
{
    public class UniversalSearchIndexManager : IUniversalSearchIndexManager
    {
        private const string FolderName = "UniversalSearch";
        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly IEndRequestTaskManager _endRequestTaskManager;
        private readonly IGetLuceneIndexSearcher _getLuceneIndexSearcher;
        private readonly IGetLuceneIndexWriter _getLuceneIndexWriter;
        private readonly IUniversalSearchItemGenerator _universalSearchItemGenerator;
        private readonly IGetSiteId _getSiteId;
        protected Analyzer Analyser;

        public UniversalSearchIndexManager(IUniversalSearchItemGenerator universalSearchItemGenerator, IGetSiteId getSiteId,
            IGetLuceneIndexWriter getLuceneIndexWriter, IGetLuceneIndexSearcher getLuceneIndexSearcher,
            IGetLuceneDirectory getLuceneDirectory, IEndRequestTaskManager endRequestTaskManager)
        {
            _universalSearchItemGenerator = universalSearchItemGenerator;
            _getSiteId = getSiteId;
            _getLuceneIndexWriter = getLuceneIndexWriter;
            _getLuceneIndexSearcher = getLuceneIndexSearcher;
            _getLuceneDirectory = getLuceneDirectory;
            _endRequestTaskManager = endRequestTaskManager;
        }

        private bool IndexExists => DirectoryReader.IndexExists(GetDirectory(_getSiteId.GetId()));

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
                _endRequestTaskManager.AddTask(new AddUniversalSearchTaskInfo(data));
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
                _endRequestTaskManager.AddTask(new AddUniversalSearchTaskInfo(data));
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
                _endRequestTaskManager.AddTask(new AddUniversalSearchTaskInfo(data));
            }
        }

        public async Task ReindexAll()
        {
            await InitializeIndex();
            await Write(async writer =>
             {
                 using (MiniProfiler.Current.Step("Reindexing"))
                 {
                     foreach (var document in await _universalSearchItemGenerator.GetAllItems())
                     {
                         writer.AddDocument(document);
                     }
                 }
             });
        }

        public void Optimise()
        {
            // optimise no longer exists - left for compatability
        }

        public async Task<IndexSearcher> GetSearcher()
        {
            await EnsureIndexExists();
            return _getLuceneIndexSearcher.Get(FolderName);
        }


        public async Task EnsureIndexExists()
        {
            if (!IndexExists)
            {
                await ReindexAll();
            }
        }

        public MrCMSIndex GetUniversalIndexInfo()
        {
            return new MrCMSIndex
            {
                DoesIndexExist = IndexExists,
                Name = "Universal Search Index",
                NumberOfDocs = GetNumberOfDocs(),
                TypeName = GetType().FullName
            };
        }

        public async Task Write(Func<IndexWriter, Task> writeFunc, bool recreateIndex = false)
        {
            if (recreateIndex)
            {
                RecreateIndex();
            }

            using (var indexWriter = _getLuceneIndexWriter.Get(FolderName, GetAnalyser()))
            {
                await writeFunc(indexWriter);
                indexWriter.Commit();
            }

            _getLuceneIndexSearcher.Reset(FolderName);
        }

        private void RecreateIndex()
        {
            _getLuceneIndexWriter.RecreateIndex(FolderName, GetAnalyser());
        }

        private bool AnyExistInEndRequest(UniversalSearchIndexData data)
        {
            return
                _endRequestTaskManager.GetTasks().OfType<AddUniversalSearchTaskInfo>()
                    .Any(task => UniversalSearchIndexData.Comparer.Equals(data, task.Data));
        }

        private int? GetNumberOfDocs()
        {
            if (!IndexExists)
            {
                return null;
            }

            using (IndexReader indexReader = DirectoryReader.Open(GetDirectory(_getSiteId.GetId())))
            {
                return indexReader.NumDocs;
            }
        }

        private async Task InitializeIndex()
        {
            await Write(writer => Task.CompletedTask, true);
        }

        public virtual Analyzer GetAnalyser()
        {
            return Analyser ?? (Analyser = new StandardAnalyzer(LuceneVersion.LUCENE_48));
        }

        private Directory GetDirectory(int siteId)
        {
            return _getLuceneDirectory.Get(siteId, FolderName);
        }
    }
}