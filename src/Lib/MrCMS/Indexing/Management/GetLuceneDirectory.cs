using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneDirectory : IGetLuceneDirectory
    {
        // TODO: Check if we should cache the standard directory, we may want to say that if we need to get to the disc level, we should always get a fresh instance
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>> DirectoryCache =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>>();

        //private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>> RamDirectoryCache =
        //    new ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>>();

        //private readonly IAzureFileSystem _azureFileSystem;
        private readonly IHostingEnvironment _hostingEnvironment;
        //private readonly FileSystemSettings _fileSystemSettings;

        //private static readonly Dictionary<int, Dictionary<string, Directory>> DirectoryCache =
        //    new Dictionary<int, Dictionary<string, Directory>>();

        public GetLuceneDirectory(
            //FileSystemSettings fileSystemSettings, 
            //IAzureFileSystem azureFileSystem,
            IHostingEnvironment hostingEnvironment
            )
        {
            //_fileSystemSettings = fileSystemSettings;
            //_azureFileSystem = azureFileSystem;
            _hostingEnvironment = hostingEnvironment;
        }

        public Directory Get(Site site, string folderName)
        {
            return GetSiteRAMCache(site)
                .GetOrAdd(folderName, s => GetDirectory(site, s));
        }

        private static ConcurrentDictionary<string, Directory> GetSiteRAMCache(Site site)
        {
            return DirectoryCache.GetOrAdd(site.Id, i => new ConcurrentDictionary<string, Directory>());
        }

        private Directory GetDirectory(Site site, string folderName)
        {
            //string location = string.Format("App_Data/Indexes/{0}/{1}/", site.Id, folderName);
            string mapPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", "Indexes",
                site.Id.ToString(), folderName);
            return new NRTCachingDirectory(FSDirectory.Open(new DirectoryInfo(mapPath)), 5, 60);
        }
    }
}