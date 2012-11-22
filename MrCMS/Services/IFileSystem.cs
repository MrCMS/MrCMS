using System.IO;

namespace MrCMS.Services
{
    public interface IFileSystem
    {
        void SaveFile(Stream stream, string filePath);
        void Delete(string filePath);
    }
}