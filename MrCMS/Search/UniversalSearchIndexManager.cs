using System;
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
using MrCMS.Indexing.Utils;
using MrCMS.Models;
using MrCMS.Search.Models;
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
            CurrentRequestData.OnEndRequest.Add(kernel =>
            {
                UniversalSearchIndexStatus status = GetStatus(entity);

                Document document = _universalSearchItemGenerator.GenerateDocument(entity);
                if (document == null)
                    return;

                Write(writer =>
                {
                    if (!status.Exists)
                    {
                        writer.AddDocument(document);
                    }
                    else
                    {
                        writer.UpdateDocument(new Term(UniversalSearchFieldNames.SearchGuid, status.Guid.ToString()),
                            document);
                    }
                }, !IndexExists);
            });
        }

        public void Delete(SystemEntity entity)
        {
            if (!_universalSearchItemGenerator.CanGenerate(entity))
            {
                return;
            }
            CurrentRequestData.OnEndRequest.Add(kernel =>
            {
                UniversalSearchIndexStatus status = GetStatus(entity);
                Write(
                    writer =>
                        writer.DeleteDocuments(new Term(UniversalSearchFieldNames.SearchGuid, status.Guid.ToString())),
                    !IndexExists);
            });
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

        public IndexSearcher GetSearcher()
        {
            return new IndexSearcher(GetDirectory(_site), true);
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

        private UniversalSearchIndexStatus GetStatus(SystemEntity entity)
        {
            if (!IndexExists)
            {
                InitializeIndex();
            }
            bool exists = false;
            Guid searchGuid = Guid.Empty;
            using (IndexSearcher indexSearcher = GetSearcher())
            {
                TopDocs topDocs =
                    indexSearcher.Search(new TermQuery(new Term(UniversalSearchFieldNames.Id, entity.Id.ToString())),
                        int.MaxValue);
                if (topDocs.ScoreDocs.Any())
                {
                    Document doc = indexSearcher.Doc(topDocs.ScoreDocs[0].Doc);
                    searchGuid = doc.GetValue<Guid>("search-guid");
                    exists = true;
                }
            }
            return new UniversalSearchIndexStatus
            {
                Exists = exists,
                Guid = searchGuid
            };
        }

        public virtual Analyzer GetAnalyser()
        {
            return Analyser ?? (Analyser = new StandardAnalyzer(Version.LUCENE_30));
        }

        private Directory GetDirectory(Site site)
        {
            return _getLuceneDirectory.Get(site, "UniversalSearch");
        }


        private void Write(Action<IndexWriter> writeFunc, bool recreateIndex = false)
        {
            using (var indexWriter = new IndexWriter(GetDirectory(_site), GetAnalyser(), recreateIndex,
                IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writeFunc(indexWriter);
            }
        }
    }
}