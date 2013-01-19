using System.IO;

namespace MrCMS.Services
{
    public interface IFileSystem
    {
        void SaveFile(Stream stream, string filePath);
        void CreateDirectory(string filePath);
        void Delete(string filePath);
        bool Exists(string filePath);
        string ApplicationPath { get; }
        string GetExtension(string fileName);
        byte[] ReadAllBytes(string location);
        string MapPath(string path);
    }
}