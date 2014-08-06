using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MrCMS.Services
{
    public interface IAzureFileSystem
    {
        CloudBlobContainer Container { get; }
        CloudStorageAccount StorageAccount { get; }
    }
}