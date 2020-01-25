using Lucene.Net.Store;
using MrCMS.Entities.Multisite;
using MrCMS.Services.Caching;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneDirectory
    {
        Directory Get(int siteId, string folderName);
    }
}