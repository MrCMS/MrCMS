using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using MrCMS.Models;

namespace MrCMS.Services
{
    public class FileSystem : IFileSystem
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public const string MediaDirectory = "/content/upload/";

        public FileSystem(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string SaveFile(Stream stream, string filePath, string contentType)
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
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                stream.CopyTo(file);
            }
            stream.Close();
            stream.Dispose();

            var relativeFilePath = GetRelativeFilePath(filePath);
            return relativeFilePath;
        }

        private string GetPath(string relativeFilePath)
        {
            relativeFilePath = GetRelativeFilePath(relativeFilePath);
            var baseDirectory = ApplicationPath;
            var path = Path.Combine(baseDirectory, relativeFilePath.TrimStart('/', '\\'));
            return path;
        }

        private static string GetRelativeFilePath(string relativeFilePath)
        {
            if (!relativeFilePath.StartsWith(MediaDirectory) && !relativeFilePath.StartsWith("/" + MediaDirectory))
                relativeFilePath = Path.Combine(MediaDirectory, relativeFilePath);
            return relativeFilePath;
        }

        public void CreateDirectory(string filePath)
        {
            var path = GetPath(filePath);

            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
        }

        public void Delete(string filePath)
        {
            var path = GetPath(filePath);

            if (Directory.Exists(path))
                Directory.Delete(path, true);
            else
                File.Delete(path);
        }

        public bool Exists(string filePath) => File.Exists(GetPath(filePath));

        public string ApplicationPath => _hostingEnvironment.WebRootPath;

        public byte[] ReadAllBytes(string filePath) => File.ReadAllBytes(GetPath(filePath));

        public Stream GetReadStream(string filePath) => File.OpenRead(GetPath(filePath));

        public void WriteToStream(string filePath, Stream stream)
        {
            var path = GetPath(filePath);
            using (var fileStream = File.OpenRead(path))
            {
                fileStream.CopyTo(stream);
            }
        }

        public IEnumerable<string> GetFiles(string filePath)
        {
            var path = GetPath(filePath);

            if (Directory.Exists(path))
            {
                foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    yield return "/" + file.Remove(0, ApplicationPath.Length).Replace('\\', '/');
            }
        }

        public CdnInfo CdnInfo => null;
    }
}