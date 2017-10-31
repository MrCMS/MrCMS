using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class AzureFileSystem : IFileSystem, IAzureFileSystem
    {
        private readonly FileSystemSettings _fileSystemSettings;
        private CloudBlobContainer _container;
        private CloudStorageAccount _storageAccount;
        private bool initialized;

        public AzureFileSystem(FileSystemSettings fileSystemSettings)
        {
            _fileSystemSettings = fileSystemSettings;
        }

        public CloudBlobContainer Container
        {
            get
            {
                EnsureInitialized();
                return _container;
            }
        }

        public CloudStorageAccount StorageAccount
        {
            get
            {
                EnsureInitialized();
                return _storageAccount;
            }
        }

        public string SaveFile(Stream stream, string filePath, string contentType)
        {
            EnsureInitialized();
            CloudBlockBlob blob = GetBlockBlobReference(filePath);
            blob.Properties.ContentType = contentType;
            blob.Properties.CacheControl = "public, max-age=31536000";
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
            blob.UploadFromStream(stream);

            return blob.Uri.ToString();
        }

        public void CreateDirectory(string filePath)
        {
        }

        public void Delete(string filePath)
        {
            EnsureInitialized();
            CloudBlockBlob blob = GetBlockBlobReference(filePath);
            try
            {
                blob.Delete();
            }
            catch
            {
            }
        }

        public bool Exists(string filePath)
        {
            EnsureInitialized();
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            CloudBlockBlob blob = GetBlockBlobReference(filePath);

            return blob.Exists();
        }

        public byte[] ReadAllBytes(string filePath)
        {
            EnsureInitialized();
            CloudBlockBlob blob = GetBlockBlobReference(filePath);
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                return memoryStream.GetBuffer();
            }
        }

        public Stream GetReadStream(string filePath)
        {
            EnsureInitialized();
            ICloudBlob blob = GetBlockBlobReference(filePath);
            return blob.OpenRead();
        }

        public void WriteToStream(string filePath, Stream stream)
        {
            EnsureInitialized();
            ICloudBlob blob = GetBlockBlobReference(filePath);
            blob.DownloadToStream(stream);
        }

        public IEnumerable<string> GetFiles(string filePath)
        {
            EnsureInitialized();
            CloudBlobDirectory cloudBlobDirectory = Container.GetDirectoryReference(filePath);

            return cloudBlobDirectory.ListBlobs().Select(item => item.Uri.ToString());
        }

        public CdnInfo CdnInfo
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_fileSystemSettings.AzureCdnDomain))
                    return new CdnInfo
                    {
                        Scheme = "https",
                        Host = _fileSystemSettings.AzureCdnDomain
                    };
                return null;
            }
        }

        private void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }
            string connectionString = _fileSystemSettings.AzureUsingEmulator
                ? string.Format("UseDevelopmentStorage=true;")
                : string.Format(
                    "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                    _fileSystemSettings.AzureAccountName, _fileSystemSettings.AzureAccountKey);
            if (CloudStorageAccount.TryParse(connectionString, out _storageAccount))
            {
                CloudBlobClient cloudBlobClient = _storageAccount.CreateCloudBlobClient();
                _container =
                    cloudBlobClient.GetContainerReference(
                        SeoHelper.TidyUrl(
                            _fileSystemSettings.AzureContainerName.RemoveInvalidUrlCharacters()));
                if (_container.CreateIfNotExists())
                {
                    _container.SetPermissions(new BlobContainerPermissions
                    {
                        PublicAccess =
                            BlobContainerPublicAccessType.Blob
                    });
                }
                initialized = true;
            }
        }

        private CloudBlockBlob GetBlockBlobReference(string filePath)
        {
            var uri = Container.Uri;
            filePath = filePath.Replace(uri + "/", "");
            return Container.GetBlockBlobReference(filePath);
        }
    }
}