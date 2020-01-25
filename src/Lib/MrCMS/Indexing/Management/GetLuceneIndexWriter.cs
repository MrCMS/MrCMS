using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Util;
using MrCMS.Entities.Multisite;
using MrCMS.Website;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneIndexWriter : IGetLuceneIndexWriter
    {
        private readonly IGetLuceneDirectory _getLuceneDirectory;
        private readonly IGetSiteId _getSiteId;

        public GetLuceneIndexWriter(IGetLuceneDirectory getLuceneDirectory, IGetSiteId getSiteId)
        {
            _getLuceneDirectory = getLuceneDirectory;
            _getSiteId = getSiteId;
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
            using (GetNewIndexWriter(definitionName, analyzer, true)) { }
        }

        private IndexWriter GetNewIndexWriter(string definitionName, Analyzer analyzer, bool recreateIndex)
        {
            var writer = new IndexWriter(_getLuceneDirectory.Get(_getSiteId.GetId(), definitionName), new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));
            if (recreateIndex)
            {
                writer.DeleteAll();
                writer.Commit();
            }
            return writer;
        }

    }
}