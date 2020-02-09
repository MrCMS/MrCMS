using Lucene.Net.Store;
using MrCMS.Entities.Multisite;
using MrCMS.Services.Caching;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneDirectory 
    {
        Directory GetRamDirectory(Site site, string folderName);
        Directory GetStandardDictionary(Site site, string folderName);
        void ResetRamDirectory(Site site, string folderName);
    }
}