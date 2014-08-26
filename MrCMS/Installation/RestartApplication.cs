using System;
using System.IO;
using System.Security;
using System.Web;
using System.Web.Hosting;
using MrCMS.Website;

namespace MrCMS.Installation
{
    public class RestartApplication : IRestartApplication
    {
        private readonly HttpContextBase _context;

        public RestartApplication(HttpContextBase context)
        {
            _context = context;
        }

        private AspNetHostingPermissionLevel? _trustLevel;

        public void Restart()
        {
            if (GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                //full trust
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                //medium trust
                bool success = TryWriteWebConfig();

                if (!success)
                {
                    throw new Exception(
                        "MrCMS needs to be restarted due to a configuration change, but was unable to do so.\r\n" +
                        "To prevent this issue in the future, a change to the web server configuration is required:\r\n" +
                        "- run the application in a full trust environment, or\r\n" +
                        "- give the application write access to the 'web.config' file.");
                }
            }
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/web.config"), CurrentRequestData.Now);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPath(string path)
        {
            return _context.Server.MapPath(path);
        }


        /// <summary>
        ///     Finds the trust level of the running application
        ///     (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        private AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (_trustLevel.HasValue)
                return _trustLevel.Value;

            //set minimum
            _trustLevel = AspNetHostingPermissionLevel.None;

            //determine maximum
            foreach (AspNetHostingPermissionLevel trustLevel in
                new[]
                {
                    AspNetHostingPermissionLevel.Unrestricted,
                    AspNetHostingPermissionLevel.High,
                    AspNetHostingPermissionLevel.Medium,
                    AspNetHostingPermissionLevel.Low,
                    AspNetHostingPermissionLevel.Minimal
                })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                    _trustLevel = trustLevel;
                    break; //we've set the highest permission we can
                }
                catch (SecurityException)
                {
                }
            }
            return _trustLevel.Value;
        }
    }
}