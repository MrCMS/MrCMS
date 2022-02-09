using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace MrCMS.Services
{
    public interface IAzureFileSystem : IFileSystem
    {
        Task<BlobContainerClient> GetContainerClient();
        Task<BlobServiceClient> GetServiceClient();
        Task<BlobClient> GetBlobClient(string filePath);
    }
}