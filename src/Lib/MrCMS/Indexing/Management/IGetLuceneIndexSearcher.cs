using Lucene.Net.Search;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneIndexSearcher 
    {
        IndexSearcher Get(IndexDefinition definition);
        IndexSearcher Get(string folderName);
        void Reset(IndexDefinition definition);
        void Reset(string folderName);
    }
}
