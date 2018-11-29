using Lucene.Net.Store;
using MrCMS.Entities.Multisite;
using System.Collections.Concurrent;
using System.IO;
using System.Web;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneDirectory : IGetLuceneDirectory
    {
        // TODO: Check if we should cache the standard directory, we may want to say that if we need to get to the disc level, we should always get a fresh instance
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>> StandardDirectoryCache =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>>();

        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>> RamDirectoryCache =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>>();

        private readonly HttpContextBase _context;

        public GetLuceneDirectory(HttpContextBase context)
        {
            _context = context;
        }

        public Directory GetRamDirectory(Site site, string folderName)
        {
            return GetSiteRAMCache(site)
                .GetOrAdd(folderName, s => new RAMDirectory(GetStandardDictionary(site, s)));
        }

        private static ConcurrentDictionary<string, Directory> GetSiteRAMCache(Site site)
        {
            return RamDirectoryCache.GetOrAdd(site.Id, i => new ConcurrentDictionary<string, Directory>());
        }

        public Directory GetStandardDictionary(Site site, string folderName)
        {
            return StandardDirectoryCache.GetOrAdd(site.Id, i => new ConcurrentDictionary<string, Directory>())
                .GetOrAdd(folderName, s => GetDirectory(site, s));
        }

        public void ResetRamDirectory(Site site, string folderName)
        {
            if (GetSiteRAMCache(site).TryRemove(folderName, out var directory))
            {
                directory.Dispose();
            }
        }

        private Directory GetDirectory(Site site, string folderName)
        {
            var location = $"~/App_Data/Indexes/{site.Id}/{folderName}/";
            var mapPath = _context.Server.MapPath(location);
            return FSDirectory.Open(new DirectoryInfo(mapPath));
        }
    }
}