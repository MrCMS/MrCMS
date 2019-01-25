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

            blob.UploadFromStreamAsync(stream).ExecuteSync();

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
                blob.DeleteAsync().ExecuteSync();
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

            return blob.ExistsAsync().ExecuteSync();
        }

        public byte[] ReadAllBytes(string filePath)
        {
            EnsureInitialized();
            CloudBlockBlob blob = GetBlockBlobReference(filePath);
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStreamAsync(memoryStream).ExecuteSync();
                return memoryStream.GetBuffer();
            }
        }

        public Stream GetReadStream(string filePath)
        {
            EnsureInitialized();
            var blob = GetBlockBlobReference(filePath);
            return blob.OpenReadAsync()
                .ExecuteSync();
        }

        public void WriteToStream(string filePath, Stream stream)
        {
            EnsureInitialized();
            var blob = GetBlockBlobReference(filePath);
            blob.DownloadToStreamAsync(stream).ExecuteSync();
        }

        public IEnumerable<string> GetFiles(string filePath)
        {
            EnsureInitialized();
            var cloudBlobDirectory = Container.GetDirectoryReference(filePath);

            var blobResultSegment = cloudBlobDirectory
                .ListBlobsSegmentedAsync(true, BlobListingDetails.None, 500, null, null, null).ExecuteSync();

            List<string> files =new List<string>();
            var continuationToken = blobResultSegment.ContinuationToken;
            files.AddRange(blobResultSegment.Results.Select(item => item.Uri.ToString()));

            while (continuationToken != null)
            {
                var resultSegment = cloudBlobDirectory.ListBlobsSegmentedAsync(continuationToken).ExecuteSync();
                files.AddRange(resultSegment.Results.Select(item => item.Uri.ToString()));
                continuationToken = resultSegment.ContinuationToken;
            }
            return files;
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
                if (_container.CreateIfNotExistsAsync().ExecuteSync())
                {
                    _container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess =
                            BlobContainerPublicAccessType.Blob
                    }).ExecuteSync();
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