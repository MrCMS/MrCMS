using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using MrCMS.Search.Models;
using MrCMS.Website;
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

        public UniversalSearchIndexManager(IUniversalSearchItemGenerator universalSearchItemGenerator, Site site, IGetLuceneDirectory getLuceneDirectory)
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
            HashSet<Document> allItems = _universalSearchItemGenerator.GetAllItems();
            Write(writer => { }, true);
            Write(writer =>
            {
                foreach (Document document in allItems)
                {
                    writer.AddDocument(document);
                }
            });
        }

        public IndexSearcher GetSearcher()
        {
            return new IndexSearcher(GetDirectory(_site), true);
        }

        private UniversalSearchIndexStatus GetStatus(SystemEntity entity)
        {
            if (!IndexExists)
            {
                Write(writer => { }, true);
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
            return _directory = _directory ?? _getLuceneDirectory.Get(site, "UniversalSearch");
        }


        private void Write(Action<IndexWriter> writeFunc, bool recreateIndex = false)
        {
            using (var indexWriter = new IndexWriter(GetDirectory(_site), GetAnalyser(), recreateIndex,
                IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writeFunc(indexWriter);
                indexWriter.Optimize();
            }
        }
    }
}