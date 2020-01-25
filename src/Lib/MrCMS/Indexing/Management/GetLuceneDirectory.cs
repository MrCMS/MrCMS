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

        private readonly IWebHostEnvironment _hostingEnvironment;

        public GetLuceneDirectory(IWebHostEnvironment hostingEnvironment
            )
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public Directory Get(int siteId, string folderName)
        {
            return GetSiteRAMCache(siteId)
                .GetOrAdd(folderName, s => GetDirectory(siteId, s));
        }

        private static ConcurrentDictionary<string, Directory> GetSiteRAMCache(int siteId)
        {
            return DirectoryCache.GetOrAdd(siteId, i => new ConcurrentDictionary<string, Directory>());
        }

        private Directory GetDirectory(int siteId, string folderName)
        {
            string mapPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", "Indexes",
                siteId.ToString(), folderName);
            return new NRTCachingDirectory(FSDirectory.Open(new DirectoryInfo(mapPath)), 5, 60);
        }
    }
}