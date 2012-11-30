using System;
using System.IO;
using System.Security.AccessControl;
using System.Web;
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
        }

        private string GetPath(string relativeFilePath)
        {
            var baseDirectory = ApplicationPath.Substring(0, ApplicationPath.Length - 1);
            var path = Path.Combine(baseDirectory, relativeFilePath);
            return path;
        }

        public void Delete(string filePath)
        {
            var path = GetPath(filePath);

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
            return MrCMSApplication.CurrentContext.Server.MapPath(path);
        }
    }
}