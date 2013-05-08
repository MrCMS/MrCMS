using System.IO;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class AzureFileSystem : IFileSystem
    {
        private readonly FileSystemSettings _fileSystemSettings;
        private readonly CloudBlobContainer _container;

        public AzureFileSystem(FileSystemSettings fileSystemSettings)
        {
            _fileSystemSettings = fileSystemSettings;

            string connectionString = _fileSystemSettings.AzureUsingEmulator
                                          ? string.Format("UseDevelopmentStorage=true;")
                                          : string.Format(
                                              "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                                              _fileSystemSettings.AzureAccountName, _fileSystemSettings.AzureAccountKey);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var container =
                cloudBlobClient.GetContainerReference(
                    SeoHelper.TidyUrl(FileService.RemoveInvalidUrlCharacters(_fileSystemSettings.AzureContainerName)));
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            _container = container;
        }

        public string SaveFile(Stream stream, string filePath, string contentType)
        {
            var blob = _container.GetBlockBlobReference(filePath);
            blob.Properties.ContentType = contentType;

            stream.Position = 0;
            blob.UploadFromStream(stream);

            return blob.Uri.ToString();
        }

        public void CreateDirectory(string filePath)
        {
        }

        public void Delete(string filePath)
        {
            var blob = _container.GetBlockBlobReference(filePath);
            try
            {
                blob.Delete();
            }
            catch { }
        }

        public bool Exists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            var blob = _container.GetBlockBlobReference(filePath);

            return blob.Exists();
        }

        public byte[] ReadAllBytes(string filePath)
        {
            var blob = _container.GetBlockBlobReference(filePath);
            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                return memoryStream.GetBuffer();
            }
        }
    }
}