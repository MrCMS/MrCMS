using System.IO;
using System.Web;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Management
{
    public class GetLuceneDirectory : IGetLuceneDirectory
    {
        private readonly IAzureFileSystem _azureFileSystem;
        private readonly HttpContextBase _context;
        private readonly FileSystemSettings _fileSystemSettings;

        public GetLuceneDirectory(FileSystemSettings fileSystemSettings, IAzureFileSystem azureFileSystem,
            HttpContextBase context)
        {
            _fileSystemSettings = fileSystemSettings;
            _azureFileSystem = azureFileSystem;
            _context = context;
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
            if (UseAzureForLucene)
            {
                string catalog = AzureDirectoryHelper.GetAzureCatalogName(site, folderName);
                return new AzureDirectory(_azureFileSystem.StorageAccount, catalog, new RAMDirectory());
            }
            string location = string.Format("~/App_Data/Indexes/{0}/{1}/", site.Id, folderName);
            string mapPath = _context.Server.MapPath(location);
            var directory = FSDirectory.Open(new DirectoryInfo(mapPath));
            return useRAMCache ? (Directory)new RAMDirectory(directory) : directory;
        }
    }
}