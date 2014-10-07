using System;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Utils;
using MrCMS.Search.Models;
using MrCMS.Website;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Search
{
    public class UniversalSearchIndexManager : IUniversalSearchIndexManager
    {
        private readonly IUniversalSearchItemGenerator _universalSearchItemGenerator;
        private readonly HttpContextBase _context;
        private readonly Site _site;
        protected Analyzer Analyser;
        private FSDirectory _directory;

        public UniversalSearchIndexManager(IUniversalSearchItemGenerator universalSearchItemGenerator, HttpContextBase context, Site site)
        {
            _universalSearchItemGenerator = universalSearchItemGenerator;
            _context = context;
            _site = site;

        }

        private UniversalSearchIndexStatus GetStatus(SystemEntity entity)
        {
            if (!IndexExists)
            {
                Write(writer => { }, true);
            }
            bool exists = false;
            Guid searchGuid = Guid.Empty;
            using (var indexSearcher = new IndexSearcher(GetDirectory(_site), true))
            {
                var topDocs = indexSearcher.Search(new TermQuery(new Term(UniversalSearchFieldNames.Id, entity.Id.ToString())), int.MaxValue);
                if (topDocs.ScoreDocs.Any())
                {
                    var doc = indexSearcher.Doc(topDocs.ScoreDocs[0].Doc);
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

        public void Index(SystemEntity entity)
        {
            if (!_universalSearchItemGenerator.CanGenerate(entity))
            {
                return;
            }
            CurrentRequestData.OnEndRequest.Add(kernel =>
            {

                var status = GetStatus(entity);

                var document = _universalSearchItemGenerator.GenerateDocument(entity);
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

        private bool IndexExists
        {
            get { return IndexReader.IndexExists(GetDirectory(_site)); }
        }

        public void Delete(SystemEntity entity)
        {
            if (!_universalSearchItemGenerator.CanGenerate(entity))
            {
                return;
            }
            CurrentRequestData.OnEndRequest.Add(kernel =>
            {
                var status = GetStatus(entity);
                Write(
                    writer =>
                        writer.DeleteDocuments(new Term(UniversalSearchFieldNames.SearchGuid, status.Guid.ToString())),
                    !IndexExists);
            });
        }

        public void ReindexAll()
        {
            var allItems = _universalSearchItemGenerator.GetAllItems();
            Write(writer => { }, true);
            Write(writer =>
            {
                foreach (var document in allItems)
                {
                    writer.AddDocument(document);
                }
            });
        }

        public virtual Analyzer GetAnalyser()
        {
            return Analyser ?? (Analyser = new StandardAnalyzer(Version.LUCENE_30));
        }

        protected Directory GetDirectory(Site site)
        {
            return _directory = _directory ?? FSDirectory.Open(new DirectoryInfo(GetLocation(site)));
        }

        public string GetLocation(Site site)
        {
            string location = string.Format("~/App_Data/Indexes/{0}/UniversalSearch/", site.Id);
            string mapPath = _context.Server.MapPath(location);
            return mapPath;
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