using System.IO;

namespace MrCMS.Services
{
    public interface IFileSystem
    {
        string SaveFile(Stream stream, string filePath, string contentType);
        void CreateDirectory(string filePath);
        void Delete(string filePath);
        bool Exists(string filePath);
        byte[] ReadAllBytes(string filePath);
        void WriteToStream(string filePath, Stream stream);
    }
}