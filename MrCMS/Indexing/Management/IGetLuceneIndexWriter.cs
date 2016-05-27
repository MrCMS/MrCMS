using Lucene.Net.Analysis;
using Lucene.Net.Index;
using MrCMS.Services.Caching;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneIndexWriter : IClearCache
    {
        IndexWriter Get(IndexDefinition definition);
        IndexWriter Get(string definitionName, Analyzer analyzer);
        void RecreateIndex(IndexDefinition definition);
        void RecreateIndex(string definitionName, Analyzer analyzer);
    }
}