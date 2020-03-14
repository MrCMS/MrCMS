using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IFileSystem
    {
        Task<string> SaveFile(Stream stream, string filePath, string contentType);
        Task CreateDirectory(string filePath);
        Task Delete(string filePath);
        Task<bool> Exists(string filePath);
        Task<byte[]> ReadAllBytes(string filePath);
        Task<Stream> GetReadStream(string filePath);
        Task WriteToStream(string filePath, Stream stream);
        Task<IEnumerable<string>> GetFiles(string filePath);
        Task<CdnInfo> GetCdnInfo();
    }
}