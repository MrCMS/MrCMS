using System.IO;
using System.Web.Hosting;
using MrCMS.Website;

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
            stream.Close();
            stream.Dispose();
        }

        private string GetPath(string relativeFilePath)
        {
            var baseDirectory = ApplicationPath.Substring(0, ApplicationPath.Length - 1);
            var path = Path.Combine(baseDirectory, relativeFilePath);
            return path;
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
                Directory.Delete(path);
            else
                File.Delete(path);
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string ApplicationPath { get { return HostingEnvironment.ApplicationPhysicalPath; } }
        public string GetExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public byte[] ReadAllBytes(string location)
        {
            return File.ReadAllBytes(location);
        }

        public string MapPath(string path)
        {
            return CurrentRequestData.CurrentContext.Server.MapPath(path);
        }
    }
}