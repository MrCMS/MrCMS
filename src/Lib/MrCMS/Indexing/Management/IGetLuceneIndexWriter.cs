using Lucene.Net.Analysis;
using Lucene.Net.Index;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneIndexWriter 
    {
        IndexWriter Get(IndexDefinition definition);
        IndexWriter Get(string definitionName, Analyzer analyzer);
        void RecreateIndex(IndexDefinition definition);
        void RecreateIndex(string definitionName, Analyzer analyzer);
    }
}