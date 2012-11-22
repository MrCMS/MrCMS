using System;
using System.IO;
using System.Security.AccessControl;

namespace MrCMS.Services
{
    public class FileSystem : IFileSystem
    {
        public void SaveFile(Stream stream, string filePath)
        {
            var path = GetPath(filePath);

            var fileInfo = new FileInfo(path);
            var directoryInfo = fileInfo.Directory;

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            using (Stream file = File.OpenWrite(path))
            {
                stream.Position = 0;
                stream.CopyTo(file);
            }
        }

        private static string GetPath(string relativeFilePath)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var baseDirectory = directory.Substring(0, directory.Length - 1);
            var path = Path.Combine(baseDirectory, relativeFilePath);
            return path;
        }

        public void Delete(string filePath)
        {
            var path = GetPath(filePath);

            File.Delete(path);
        }
    }
}