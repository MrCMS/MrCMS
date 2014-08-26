using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Web;

namespace MrCMS.Installation
{
    public class FileSystemAccessService : IFileSystemAccessService
    {
        private readonly HttpContextBase _context;

        public FileSystemAccessService(HttpContextBase context)
        {
            _context = context;
        }

        public InstallationResult EnsureAccessToFileSystem()
        {
            var result = new InstallationResult();
            //validate permissions
            string rootDir = _context.Server.MapPath("~/");
            var dirsToCheck = new List<string>();
            dirsToCheck.Add(rootDir);
            dirsToCheck.Add(rootDir + "App_Data");
            dirsToCheck.Add(rootDir + "bin");
            dirsToCheck.Add(rootDir + "content");
            dirsToCheck.Add(rootDir + "content/upload");
            foreach (string dir in dirsToCheck)
                if (!CheckPermissions(new RequiredCheck(false, true, true, true), dir))
                    result.AddModelError(
                        String.Format(
                            "The '{0}' account is not granted with Modify permission on folder '{1}'. Please configure these permissions.",
                            WindowsIdentity.GetCurrent().Name, dir));
                else
                {
                    var directoryInfo = new DirectoryInfo(dir);
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                }


            var filesToCheck = new List<string>();
            filesToCheck.Add(rootDir + "web.config");
            filesToCheck.Add(rootDir + "ConnectionStrings.config");
            foreach (string file in filesToCheck)
                if (!CheckPermissions(new RequiredCheck(false, true, true, true), file))
                    result.AddModelError(
                        String.Format(
                            "The '{0}' account is not granted with Modify permission on file '{1}'. Please configure these permissions.",
                            WindowsIdentity.GetCurrent().Name, file));
            return result;
        }

        public void EmptyAppData()
        {
            string appData = _context.Server.MapPath("~/App_Data");
            var directoryInfo = new DirectoryInfo(appData);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        /// <summary>
        ///     Check permissions
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="checkRead">Check read</param>
        /// <param name="checkWrite">Check write</param>
        /// <param name="checkModify">Check modify</param>
        /// <param name="checkDelete">Check delete</param>
        /// <returns>Resulr</returns>
        private static bool CheckPermissions(RequiredCheck requiredCheck, string path)
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            try
            {
                var rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier));
                var permissionsChecker = new PermissionsChecker(current, rules, requiredCheck);
                return permissionsChecker.IsValid();
            }
            catch
            {
                return true;
            }
        }
    }
}