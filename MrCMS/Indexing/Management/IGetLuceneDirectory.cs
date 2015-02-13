using Lucene.Net.Store;
using MrCMS.Entities.Multisite;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneDirectory
    {
        Directory Get(Site site, string folderName, bool useRAMCache = false);
    }
}