using Lucene.Net.Search;
//using MrCMS.Services.Caching;

namespace MrCMS.Indexing.Management
{
    // TODO: clear cache
    public interface IGetLuceneIndexSearcher //: IClearCache
    {
        IndexSearcher Get(IndexDefinition definition);
        IndexSearcher Get(string folderName);
        void Reset(IndexDefinition definition);
        void Reset(string folderName);
    }
}
