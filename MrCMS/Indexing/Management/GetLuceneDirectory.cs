using System.Collections.Generic;
using System.IO;
using System.Web;
using Lucene.Net.Store;
using MrCMS.Entities.Multisite;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneDirectory : IGetLuceneDirectory
    {
        private static readonly Dictionary<int, Dictionary<string, Directory>> StandardDirectoryCache =
            new Dictionary<int, Dictionary<string, Directory>>();

        private static readonly Dictionary<int, Dictionary<string, Directory>> RamDirectoryCache =
            new Dictionary<int, Dictionary<string, Directory>>();

        private readonly HttpContextBase _context;

        public GetLuceneDirectory(HttpContextBase context)
        {
            _context = context;
        }

        public Directory GetRamDirectory(Site site, string folderName)
        {
            var siteId = site.Id;
            if (!RamDirectoryCache.ContainsKey(siteId))
                RamDirectoryCache[siteId] = new Dictionary<string, Directory>();
            var dictionary = RamDirectoryCache[siteId];
            if (!dictionary.ContainsKey(folderName))
                dictionary[folderName] = new RAMDirectory(GetStandardDictionary(site, folderName));
            return dictionary[folderName];
        }

        public Directory GetStandardDictionary(Site site, string folderName)
        {
            var siteId = site.Id;
            if (!StandardDirectoryCache.ContainsKey(siteId))
                StandardDirectoryCache[siteId] = new Dictionary<string, Directory>();
            var dictionary = StandardDirectoryCache[siteId];
            if (!dictionary.ContainsKey(folderName)) dictionary[folderName] = GetDirectory(site, folderName);
            return dictionary[folderName];
        }

        public void ResetRamDirectory(Site site, string folderName)
        {
            var siteId = site.Id;
            if (!RamDirectoryCache.ContainsKey(siteId)) return;
            var dictionary = RamDirectoryCache[siteId];
            if (!dictionary.ContainsKey(folderName)) return;

            var directory = dictionary[folderName];
            directory.Dispose();
            dictionary.Remove(folderName);
        }

        private Directory GetDirectory(Site site, string folderName)
        {
            var location = $"~/App_Data/Indexes/{site.Id}/{folderName}/";
            var mapPath = _context.Server.MapPath(location);
            return FSDirectory.Open(new DirectoryInfo(mapPath));
        }
    }
}