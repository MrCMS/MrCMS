using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MrCMS.Services
{
    public interface IAzureFileSystem
    {
        Task<CloudBlobContainer> GetContainer();
        Task<CloudStorageAccount> GetStorageAccount();
    }
}