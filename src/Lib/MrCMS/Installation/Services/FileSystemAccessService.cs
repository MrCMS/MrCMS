using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public class FileSystemAccessService : IFileSystemAccessService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileSystemAccessService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public InstallationResult EnsureAccessToFileSystem()
        {
            var result = new InstallationResult();
            ////validate permissions
            var dirsToCheck = new List<string>();
            dirsToCheck.Add("App_Data");
            foreach (string dir in dirsToCheck)
            {
                var directoryInfo = new DirectoryInfo(Path.Combine(_hostingEnvironment.ContentRootPath, dir));
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
            }

            return result;
        }

        public void EmptyAppData()
        {
            string appData = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data");
            var directoryInfo = new DirectoryInfo(appData);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
            //AppDataSystemConfigurationProvider.ClearCache();
            //AppDataConfigurationProvider.ClearCache();
        }

        ///// <summary>
        /////     Check permissions
        ///// </summary>
        ///// <param name="path">Path</param>
        ///// <param name="checkRead">Check read</param>
        ///// <param name="checkWrite">Check write</param>
        ///// <param name="checkModify">Check modify</param>
        ///// <param name="checkDelete">Check delete</param>
        ///// <returns>Resulr</returns>
        //private static bool CheckPermissions(RequiredCheck requiredCheck, string path)
        //{
        //    WindowsIdentity current = WindowsIdentity.GetCurrent();
        //    try
        //    {
        //        var rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier));
        //        var permissionsChecker = new PermissionsChecker(current, rules, requiredCheck);
        //        return permissionsChecker.IsValid();
        //    }
        //    catch
        //    {
        //        return true;
        //    }
        //}
    }
}