using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        public async Task<string> SaveFile(Stream stream, string filePath, string contentType)
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

                await stream.CopyToAsync(file);
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

        public Task CreateDirectory(string filePath)
        {
            var path = GetPath(filePath);

            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            return Task.CompletedTask;
        }

        public Task Delete(string filePath)
        {
            var path = GetPath(filePath);

            if (Directory.Exists(path))
                Directory.Delete(path, true);
            else
                File.Delete(path);

            return Task.CompletedTask;
        }

        public Task<bool> Exists(string filePath) => Task.FromResult(File.Exists(GetPath(filePath)));

        public string ApplicationPath => _hostingEnvironment.WebRootPath;

        public Task<byte[]> ReadAllBytes(string filePath) => File.ReadAllBytesAsync(GetPath(filePath));

        public Task<Stream> GetReadStream(string filePath) => Task.FromResult<Stream>(File.OpenRead(GetPath(filePath)));

        public Task WriteToStream(string filePath, Stream stream)
        {
            var path = GetPath(filePath);
            using var fileStream = File.OpenRead(path);
            return fileStream.CopyToAsync(stream);
        }

        public Task<IEnumerable<string>> GetFiles(string filePath)
        {
            var path = GetPath(filePath);

            var result = new List<string>();
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    result.Add("/" + file.Remove(0, ApplicationPath.Length).Replace('\\', '/'));
            }

            return Task.FromResult<IEnumerable<string>>(result);
        }

        public Task<CdnInfo> GetCdnInfo() => Task.FromResult<CdnInfo>(null);
    }
}