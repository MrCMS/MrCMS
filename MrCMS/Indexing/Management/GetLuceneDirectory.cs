using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
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
        private readonly IAzureFileSystem _azureFileSystem;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly FileSystemSettings _fileSystemSettings;

        private static readonly Dictionary<int, Dictionary<string, Directory>> DirectoryCache =
            new Dictionary<int, Dictionary<string, Directory>>();

        public GetLuceneDirectory(FileSystemSettings fileSystemSettings, IAzureFileSystem azureFileSystem,
            IHostingEnvironment hostingEnvironment)
        {
            _fileSystemSettings = fileSystemSettings;
            _azureFileSystem = azureFileSystem;
            _hostingEnvironment = hostingEnvironment;
        }


        private bool UseAzureForLucene
        {
            get
            {
                return _fileSystemSettings.StorageType == typeof(AzureFileSystem).FullName &&
                       _fileSystemSettings.UseAzureForLucene;
            }
        }

        public Directory Get(Site site, string folderName, bool useRAMCache = false)
        {
            var siteId = site.Id;
            if (!DirectoryCache.ContainsKey(siteId))
            {
                DirectoryCache[siteId] = new Dictionary<string, Directory>();
            }
            var dictionary = DirectoryCache[siteId];
            if (!dictionary.ContainsKey(folderName))
            {
                dictionary[folderName] = GetDirectory(site, folderName, useRAMCache);
            }
            return dictionary[folderName];
        }

        private Directory GetDirectory(Site site, string folderName, bool useRAMCache)
        {
            if (UseAzureForLucene)
            {
                string catalog = AzureDirectoryHelper.GetAzureCatalogName(site, folderName);
                return new AzureDirectory(_azureFileSystem.StorageAccount, catalog, new RAMDirectory());
            }
            string location = string.Format("BApp_Data/Indexes/{0}/{1}/", site.Id, folderName);
            string mapPath = Path.Combine(_hostingEnvironment.ContentRootPath, location);
            var directory = FSDirectory.Open(new DirectoryInfo(mapPath));
            return useRAMCache ? (Directory)new RAMDirectory(directory) : directory;
        }

        public void ClearCache()
        {
            foreach (var indexSearcher in DirectoryCache.SelectMany(x => x.Value.Values))
                indexSearcher.Dispose();

            DirectoryCache.Clear();
        }
    }
}