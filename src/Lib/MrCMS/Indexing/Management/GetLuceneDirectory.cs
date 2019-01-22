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
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>> DirectoryCache =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, Directory>>();

        private readonly IHostingEnvironment _hostingEnvironment;

        public GetLuceneDirectory(
            IHostingEnvironment hostingEnvironment
            )
        {
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
            string mapPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", "Indexes",
                site.Id.ToString(), folderName);
            return new NRTCachingDirectory(FSDirectory.Open(new DirectoryInfo(mapPath)), 5, 60);
        }
    }
}