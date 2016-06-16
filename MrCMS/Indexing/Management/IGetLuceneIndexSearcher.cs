using Lucene.Net.Search;
using MrCMS.Services.Caching;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneIndexSearcher : IClearCache
    {
        IndexSearcher Get(IndexDefinition definition);
        IndexSearcher Get(string folderName);
        void Reset(IndexDefinition definition);
        void Reset(string folderName);
    }
}
