using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class AzureFileSystem : IFileSystem, IAzureFileSystem
    {
        private FileSystemSettings _fileSystemSettings;
        private CloudBlobContainer _container;
        private CloudStorageAccount _storageAccount;
        bool initialized;

        public AzureFileSystem(FileSystemSettings fileSystemSettings)
        {
            _fileSystemSettings = fileSystemSettings;
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
                var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
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
                return _storageAccount;
            }
        }

        public string SaveFile(Stream stream, string filePath, string contentType)
        {
            EnsureInitialized();
            var blob = Container.GetBlockBlobReference(filePath);
            blob.Properties.ContentType = contentType;

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
            var blob = Container.GetBlockBlobReference(filePath);
            try
            {
                blob.Delete();
            }
            catch { }
        }

        public bool Exists(string filePath)
        {
            EnsureInitialized();
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            var blob = Container.GetBlockBlobReference(filePath);

            return blob.Exists();
        }

        public byte[] ReadAllBytes(string filePath)
        {
            EnsureInitialized();
            var blob = Container.GetBlockBlobReference(filePath);
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                return memoryStream.GetBuffer();
            }
        }

        public Stream GetReadStream(string filePath)
        {
            EnsureInitialized();
            ICloudBlob blob = Container.GetBlockBlobReference(filePath);
            return blob.OpenRead();
        }

        public void WriteToStream(string filePath, Stream stream)
        {
            EnsureInitialized();
            ICloudBlob blob = Container.GetBlockBlobReference(filePath);
            blob.DownloadToStream(stream);
        }

        public IEnumerable<string> GetFiles(string filePath)
        {
            EnsureInitialized();
            var cloudBlobDirectory = Container.GetDirectoryReference(filePath);

            return cloudBlobDirectory.ListBlobs().Select(item => item.Uri.ToString());
        }
    }
}