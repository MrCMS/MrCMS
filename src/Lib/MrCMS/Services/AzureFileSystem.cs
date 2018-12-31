using System;
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

            // TODO: async
            blob.UploadFromStreamAsync(stream).GetAwaiter().GetResult();

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
                // TODO: async
                blob.DeleteAsync().GetAwaiter().GetResult();
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

            // TODO: async
            return blob.ExistsAsync().GetAwaiter().GetResult();
        }

        public byte[] ReadAllBytes(string filePath)
        {
            EnsureInitialized();
            CloudBlockBlob blob = GetBlockBlobReference(filePath);
            using (var memoryStream = new MemoryStream())
            {
                // TODO: async
                blob.DownloadToStreamAsync(memoryStream).GetAwaiter().GetResult();
                return memoryStream.GetBuffer();
            }
        }

        public Stream GetReadStream(string filePath)
        {
            EnsureInitialized();
            ICloudBlob blob = GetBlockBlobReference(filePath);
            // TODO: async
            throw new NotImplementedException("Not yet implemented");
            //return blob.OpenReadAsync().GetAwaiter().GetResult();
        }

        public void WriteToStream(string filePath, Stream stream)
        {
            EnsureInitialized();
            ICloudBlob blob = GetBlockBlobReference(filePath);
            // TODO: async
            blob.DownloadToStreamAsync(stream).GetAwaiter().GetResult();
        }

        public IEnumerable<string> GetFiles(string filePath)
        {
            EnsureInitialized();
            CloudBlobDirectory cloudBlobDirectory = Container.GetDirectoryReference(filePath);

            // TODO: async
            throw new NotImplementedException("Not yet implemented");
            //var blobResultSegment = cloudBlobDirectory.ListBlobsSegmentedAsync().GetAwaiter().GetResult();  
            //return blobResultSegment.Select(item => item.Uri.ToString());
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
                // TODO: async
                if (_container.CreateIfNotExistsAsync().GetAwaiter().GetResult())
                {
                    _container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess =
                            BlobContainerPublicAccessType.Blob
                    }).GetAwaiter().GetResult();
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