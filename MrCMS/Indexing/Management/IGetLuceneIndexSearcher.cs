using Lucene.Net.Search;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneIndexSearcher
    {
        IndexSearcher Get(string folderName);
        int SiteId { get; }
    }
}