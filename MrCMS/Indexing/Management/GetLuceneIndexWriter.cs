using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Util;
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
            var writer = new IndexWriter(_getLuceneDirectory.Get(_site, definitionName), new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));
            // TODO: recreate index
            return writer;
            //analyzer, recreateIndex, IndexWriter.MAX_TERM_LENGTH);
        }

        public void ClearCache()
        {
            //foreach (var indexSearcher in Writers.SelectMany(x => x.Value.Values))
            //    indexSearcher.Dispose();

            //Writers.Clear();
        }
    }
}