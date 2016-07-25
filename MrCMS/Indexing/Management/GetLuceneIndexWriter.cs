using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using MrCMS.Entities.Multisite;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneIndexWriter : IGetLuceneIndexWriter
    {
        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly Site _site;

        public GetLuceneIndexWriter(IGetLuceneDirectory getLuceneDirectory, Site site)
        {
            _getLuceneDirectory = getLuceneDirectory;
            _site = site;
        }

        public IndexWriter Get(IndexDefinition definition)
        {
            return Get(definition.IndexFolderName, definition.GetAnalyser());
        }

        public IndexWriter Get(string definitionName, Analyzer analyzer)
        {
            return GetNewIndexWriter(definitionName, analyzer, false);
        }

        public void RecreateIndex(IndexDefinition definition)
        {
            RecreateIndex(definition.IndexFolderName, definition.GetAnalyser());
        }

        public void RecreateIndex(string definitionName, Analyzer analyzer)
        {
            //if (Writers.ContainsKey(_site.Id) && Writers[_site.Id].ContainsKey(definitionName))
            //{
            //    var existing = Writers[_site.Id][definitionName];
            //    if (existing != null) existing.Dispose();
            //    Writers[_site.Id].Remove(definitionName);
            //}
            using (GetNewIndexWriter(definitionName, analyzer, true)) { }
        }

        private IndexWriter GetNewIndexWriter(string definitionName, Analyzer analyzer, bool recreateIndex)
        {
            return new IndexWriter(_getLuceneDirectory.Get(_site, definitionName), analyzer, recreateIndex, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public void ClearCache()
        {
            //foreach (var indexSearcher in Writers.SelectMany(x => x.Value.Values))
            //    indexSearcher.Dispose();

            //Writers.Clear();
        }
    }
}