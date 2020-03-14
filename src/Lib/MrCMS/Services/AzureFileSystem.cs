using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class AzureFileSystem : IFileSystem, IAzureFileSystem
    {
        private readonly IConfigurationProvider _configurationProvider;
        private CloudBlobContainer _container;
        private CloudStorageAccount _storageAccount;
        private bool _initialized;

        public AzureFileSystem(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task<CloudBlobContainer> GetContainer()
        {
            await EnsureInitialized();
            return _container;
        }

        public async Task<CloudStorageAccount> GetStorageAccount()
        {
            await EnsureInitialized();
            return _storageAccount;
        }

        public async Task<string> SaveFile(Stream stream, string filePath, string contentType)
        {
            await EnsureInitialized();
            CloudBlockBlob blob = await GetBlockBlobReference(filePath);
            blob.Properties.ContentType = contentType;
            blob.Properties.CacheControl = "public, max-age=31536000";
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            await blob.UploadFromStreamAsync(stream);

            return blob.Uri.ToString();
        }

        public Task CreateDirectory(string filePath)
        {
            return Task.CompletedTask;
        }

        public async Task Delete(string filePath)
        {
            await EnsureInitialized();
            CloudBlockBlob blob = await GetBlockBlobReference(filePath);
            try
            {
                await blob.DeleteAsync();
            }
            catch
            {
            }
        }

        public async Task<bool> Exists(string filePath)
        {
            await EnsureInitialized();
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            CloudBlockBlob blob = await GetBlockBlobReference(filePath);

            return await blob.ExistsAsync();
        }

        public async Task<byte[]> ReadAllBytes(string filePath)
        {
            await EnsureInitialized();
            CloudBlockBlob blob = await GetBlockBlobReference(filePath);
            await using var memoryStream = new MemoryStream();
            await blob.DownloadToStreamAsync(memoryStream);
            return memoryStream.GetBuffer();
        }

        public async Task<Stream> GetReadStream(string filePath)
        {
            await EnsureInitialized();
            var blob = await GetBlockBlobReference(filePath);
            return await blob.OpenReadAsync();
        }

        public async Task WriteToStream(string filePath, Stream stream)
        {
            await EnsureInitialized();
            var blob = await GetBlockBlobReference(filePath);
            await blob.DownloadToStreamAsync(stream);
        }

        public async Task<IEnumerable<string>> GetFiles(string filePath)
        {
            await EnsureInitialized();
            var container = await GetContainer();
            var cloudBlobDirectory = container.GetDirectoryReference(filePath);

            var blobResultSegment = cloudBlobDirectory
                .ListBlobsSegmentedAsync(true, BlobListingDetails.None, 500, null, null, null).ExecuteSync();

            List<string> files = new List<string>();
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

        public async Task<CdnInfo> GetCdnInfo()
        {
            var fileSystemSettings = await _configurationProvider.GetSiteSettings<FileSystemSettings>();
            if (!string.IsNullOrWhiteSpace(fileSystemSettings.AzureCdnDomain))
                return new CdnInfo
                {
                    Scheme = "https",
                    Host = fileSystemSettings.AzureCdnDomain
                };
            return null;
        }

        private async Task EnsureInitialized()
        {
            if (_initialized)
            {
                return;
            }
            var fileSystemSettings = await _configurationProvider.GetSiteSettings<FileSystemSettings>();
            string connectionString = fileSystemSettings.AzureUsingEmulator
                ? string.Format("UseDevelopmentStorage=true;")
                : string.Format(
                    "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                    fileSystemSettings.AzureAccountName, fileSystemSettings.AzureAccountKey);
            if (CloudStorageAccount.TryParse(connectionString, out _storageAccount))
            {
                CloudBlobClient cloudBlobClient = _storageAccount.CreateCloudBlobClient();
                _container =
                    cloudBlobClient.GetContainerReference(
                        SeoHelper.TidyUrl(
                            fileSystemSettings.AzureContainerName.RemoveInvalidUrlCharacters()));
                if (_container.CreateIfNotExistsAsync().ExecuteSync())
                {
                    _container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess =
                            BlobContainerPublicAccessType.Blob
                    }).ExecuteSync();
                }
                _initialized = true;
            }
        }

        private async Task<CloudBlockBlob> GetBlockBlobReference(string filePath)
        {
            var container = await GetContainer();
            var uri = container.Uri;
            filePath = filePath.Replace(uri + "/", "");
            return container.GetBlockBlobReference(filePath);
        }
    }
}