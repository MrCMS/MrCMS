using Lucene.Net.Store;
using MrCMS.Entities.Multisite;

namespace MrCMS.Indexing.Management
{
    // TODO: clear cache
    public interface IGetLuceneDirectory //: IClearCache
    {
        Directory Get(Site site, string folderName);
    }
}