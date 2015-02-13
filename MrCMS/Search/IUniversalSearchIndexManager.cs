using Lucene.Net.Search;
using MrCMS.Entities;
using MrCMS.Models;

namespace MrCMS.Search
{
    public interface IUniversalSearchIndexManager
    {
        void Index(SystemEntity entity);
        void Delete(SystemEntity entity);
        void ReindexAll();
        IndexSearcher GetSearcher();

        MrCMSIndex GetUniversalIndexInfo();
    }
}