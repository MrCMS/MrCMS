using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using FluentNHibernate.Cfg.Db;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using MySql.Data.MySqlClient;
using NHibernate;
using AuthorizationRuleCollection = System.Security.AccessControl.AuthorizationRuleCollection;

namespace MrCMS.Installation
{
    public class InstallationService : IInstallationService
    {
        private static AspNetHostingPermissionLevel? _trustLevel;
        private readonly HttpContextBase _context;

        public InstallationService(HttpContextBase context)
        {
            _context = context;
        }

        public InstallationResult Install(InstallModel model)
        {
            var result = new InstallationResult();

            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            //SQL Server

            switch (model.DatabaseType)
            {
                case DatabaseType.Sqlite:
                    break;
                case DatabaseType.MsSql:
                    if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw",
                                                       StringComparison.InvariantCultureIgnoreCase))
                    {
                        //raw connection string
                        if (string.IsNullOrEmpty(model.DatabaseConnectionString))
                            result.AddModelError("A SQL connection string is required");

                        try
                        {
                            //try to create connection string
                            new SqlConnectionStringBuilder(model.DatabaseConnectionString);
                        }
                        catch
                        {
                            result.AddModelError("Wrong SQL connection string format");
                        }
                    }
                    else
                    {
                        //values
                        if (string.IsNullOrEmpty(model.SqlServerName))
                            result.AddModelError("SQL Server name is required");
                        if (string.IsNullOrEmpty(model.SqlDatabaseName))
                            result.AddModelError("Database name is required");

                        //authentication type
                        if (model.SqlAuthenticationType.Equals("sqlauthentication",
                                                               StringComparison.InvariantCultureIgnoreCase))
                        {
                            //SQL authentication
                            if (string.IsNullOrEmpty(model.SqlServerUsername))
                                result.AddModelError("SQL Username is required");
                            if (string.IsNullOrEmpty(model.SqlServerPassword))
                                result.AddModelError("SQL Password is required");
                        }
                    }
                    break;
                case DatabaseType.MySQL:
                    if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw",
                                                       StringComparison.InvariantCultureIgnoreCase))
                    {
                        //raw connection string
                        if (string.IsNullOrEmpty(model.DatabaseConnectionString))
                            result.AddModelError("A SQL connection string is required");

                        try
                        {
                            //try to create connection string
                            new MySqlConnectionStringBuilder(model.DatabaseConnectionString);
                        }
                        catch
                        {
                            result.AddModelError("Wrong SQL connection string format");
                        }
                    }
                    else
                    {
                        //values
                        if (string.IsNullOrEmpty(model.SqlServerName))
                            result.AddModelError("SQL Server name is required");
                        if (string.IsNullOrEmpty(model.SqlDatabaseName))
                            result.AddModelError("Database name is required");

                        //authentication type
                        if (model.SqlAuthenticationType.Equals("sqlauthentication",
                                                               StringComparison.InvariantCultureIgnoreCase))
                        {
                            //SQL authentication
                            if (string.IsNullOrEmpty(model.SqlServerUsername))
                                result.AddModelError("SQL Username is required");
                            if (string.IsNullOrEmpty(model.SqlServerPassword))
                                result.AddModelError("SQL Password is required");
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Consider granting access rights to the resource to the ASP.NET request identity. 
            //ASP.NET has a base process identity 
            //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
            //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
            //If the application is impersonating via <identity impersonate="true"/>, 
            //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.

            //validate permissions
            string rootDir = _context.Server.MapPath("~/");
            var dirsToCheck = new List<string>();
            dirsToCheck.Add(rootDir);
            dirsToCheck.Add(rootDir + "App_Data");
            dirsToCheck.Add(rootDir + "bin");
            dirsToCheck.Add(rootDir + "content");
            dirsToCheck.Add(rootDir + "content/upload");
            foreach (string dir in dirsToCheck)
                if (!checkPermissions(dir, false, true, true, true))
                    result.AddModelError(
                        string.Format(
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
            foreach (string file in filesToCheck)
                if (!checkPermissions(file, false, true, true, true))
                    result.AddModelError(
                        string.Format(
                            "The '{0}' account is not granted with Modify permission on file '{1}'. Please configure these permissions.",
                            WindowsIdentity.GetCurrent().Name, file));

            if (result.Success)
            {
                try
                {
                    string connectionString = null;
                    switch (model.DatabaseType)
                    {
                        case DatabaseType.Sqlite:
                            //SQLite
                            string databaseFileName = "MrCMS.Db.db";
                            string databasePath = @"|DataDirectory|\" + databaseFileName;
                            connectionString = "Data Source=" + databasePath;

                            //drop database if exists
                            string databaseFullPath = HostingEnvironment.MapPath("~/App_Data/") + databaseFileName;
                            if (File.Exists(databaseFullPath))
                            {
                                File.Delete(databaseFullPath);
                            }
                            using (File.Create(databaseFullPath))
                            {
                            }
                            break;
                        case DatabaseType.MsSql:
                            if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw",
                                                               StringComparison.InvariantCultureIgnoreCase))
                            {
                                //raw connection string
                                connectionString = model.DatabaseConnectionString;
                            }
                            else
                            {
                                //values
                                connectionString =
                                    createConnectionString(model.SqlAuthenticationType == "windowsauthentication",
                                                           model.SqlServerName, model.SqlDatabaseName,
                                                           model.SqlServerUsername, model.SqlServerPassword);
                            }

                            if (model.SqlServerCreateDatabase)
                            {
                                if (!sqlServerDatabaseExists(connectionString))
                                {
                                    //create database
                                    string errorCreatingDatabase = createSqlDatabase(connectionString);
                                    if (!String.IsNullOrEmpty(errorCreatingDatabase))
                                        throw new Exception(errorCreatingDatabase);
                                    else
                                    {
                                        //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
                                        //that's just wait 3 seconds
                                        Thread.Sleep(3000);
                                    }
                                }
                            }
                            else
                            {
                                //check whether database exists
                                if (!sqlServerDatabaseExists(connectionString))
                                    throw new Exception(
                                        "Database does not exist or you don't have permissions to connect to it");
                            }
                            break;
                        case DatabaseType.MySQL:
                            if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw",
                                                               StringComparison.InvariantCultureIgnoreCase))
                            {
                                //raw connection string
                                connectionString = model.DatabaseConnectionString;
                            }
                            else
                            {
                                //values
                                connectionString =
                                    createMySqlConnectionString(model.SqlAuthenticationType == "windowsauthentication",
                                                                model.SqlServerName, model.SqlDatabaseName,
                                                                model.SqlServerUsername, model.SqlServerPassword);
                            }

                            if (model.SqlServerCreateDatabase)
                            {
                                if (!mySqlServerDatabaseExists(connectionString))
                                {
                                    //create database
                                    string errorCreatingDatabase = createMySqlDatabase(connectionString);
                                    if (!String.IsNullOrEmpty(errorCreatingDatabase))
                                        throw new Exception(errorCreatingDatabase);
                                    else
                                    {
                                        //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
                                        //that's just wait 3 seconds
                                        Thread.Sleep(3000);
                                    }
                                }
                            }
                            else
                            {
                                //check whether database exists
                                if (!mySqlServerDatabaseExists(connectionString))
                                    throw new Exception(
                                        "Database does not exist or you don't have permissions to connect to it");
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                    //save settings
                    SetUpInitialData(model, connectionString, model.DatabaseType);
                    
                    var connectionStringSettings = new ConnectionStringSettings("mrcms", connectionString)
                        {
                            ProviderName = GetProviderName(model.DatabaseType)

                        };
                    Configuration cfg = WebConfigurationManager.OpenWebConfiguration(@"/");
                    cfg.ConnectionStrings.ConnectionStrings.Remove("mrcms");
                    cfg.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);
                    cfg.Save();

                    RestartAppDomain();
                }
                catch (Exception exception)
                {
                    result.AddModelError("Setup failed: " + exception);
                }
            }

            return result;
        }

        private string GetProviderName(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.Sqlite:
                    return "System.Data.SQLite";
                case DatabaseType.MsSql:
                    return "System.Data.SqlClient";
                case DatabaseType.MySQL:
                    return "MySql.Data.MySqlClient";
                default:
                    throw new ArgumentOutOfRangeException("databaseType");
            }
        }

        private void SetUpInitialData(InstallModel model, string connectionString, DatabaseType databaseType)
        {
            IPersistenceConfigurer connection;
            switch (databaseType)
            {
                case DatabaseType.Sqlite:
                    connection = SQLiteConfiguration.Standard.ConnectionString(connectionString);
                    break;
                case DatabaseType.MsSql:
                    connection = MsSqlConfiguration.MsSql2008.ConnectionString(connectionString);
                    break;
                case DatabaseType.MySQL:
                    connection = MySQLConfiguration.Standard.ConnectionString(connectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("databaseType");
            }
            var configurator = new NHibernateConfigurator
                {
                    CacheEnabled = true,
                    PersistenceOverride = connection
                };

            ISessionFactory sessionFactory = configurator.CreateSessionFactory();
            ISession session = sessionFactory.OpenSession();

            var siteService = new SiteService(session, _context.Request);
            //settings
            var mediaSettings = new MediaSettings();

            var site = new Site { Name = model.SiteName, BaseUrl = model.SiteUrl };
            session.Transact(sess => sess.Save(site));
            CurrentRequestData.CurrentSite = site;
            var currentSite = new CurrentSite(site);

            var siteSettings = new SiteSettings { Site = site };

            var documentService = new DocumentService(session, siteSettings, currentSite);
            var layoutAreaService = new LayoutAreaService(session);

            var user = new User
                {
                    Email = model.AdminEmail,
                    IsActive = true
                };

            var authorisationService = new AuthorisationService();
            authorisationService.ValidatePassword(model.AdminPassword, model.ConfirmPassword);
            authorisationService.SetPassword(user, model.AdminPassword, model.ConfirmPassword);
            session.Transact(sess => sess.Save(user));
            CurrentRequestData.CurrentUser = user;

            documentService.AddDocument(model.BaseLayout);
            var layoutAreas = new List<LayoutArea>
                                  {
                                      new LayoutArea
                                          {
                                              AreaName = "Main Navigation",
                                              CreatedOn = DateTime.Now,
                                              Layout = model.BaseLayout,
                                              Site = site
                                          },
                                      new LayoutArea
                                          {
                                              AreaName = "Before Content",
                                              CreatedOn = DateTime.Now,
                                              Layout = model.BaseLayout,
                                              Site = site
                                          },
                                      new LayoutArea
                                          {
                                              AreaName = "After Content",
                                              CreatedOn = DateTime.Now,
                                              Layout = model.BaseLayout,
                                              Site = site
                                          }
                                  };

            foreach (LayoutArea l in layoutAreas)
                layoutAreaService.SaveArea(l);

            documentService.AddDocument(model.HomePage);
            documentService.AddDocument(model.Page2);
            documentService.AddDocument(model.Page3);
            documentService.AddDocument(model.Error403);
            documentService.AddDocument(model.Error404);

            documentService.AddDocument(model.Error500);

            var defaultMediaCategory = new MediaCategory
                {
                    Name = "Default",
                    UrlSegment = "default",
                    Site = site
                };
            documentService.AddDocument(defaultMediaCategory);

            siteSettings.DefaultLayoutId = model.BaseLayout.Id;
            siteSettings.Error403PageId = model.Error403.Id;
            siteSettings.Error404PageId = model.Error404.Id;
            siteSettings.Error500PageId = model.Error500.Id;

            siteSettings.EnableInlineEditing = true;
            siteSettings.SiteIsLive = true;
            
            mediaSettings.ThumbnailImageHeight = 50;
            mediaSettings.ThumbnailImageWidth = 50;
            mediaSettings.LargeImageHeight = 800;
            mediaSettings.LargeImageWidth = 800;
            mediaSettings.MediumImageHeight = 500;
            mediaSettings.MediumImageWidth = 500;
            mediaSettings.SmallImageHeight = 200;
            mediaSettings.SmallImageWidth = 200;

            var configurationProvider = new ConfigurationProvider(new SettingService(session),
                                                                  currentSite);
            configurationProvider.SaveSettings(siteSettings);
            configurationProvider.SaveSettings(mediaSettings);


            var adminUserRole = new UserRole
                {
                    Name = UserRole.Administrator
                };

            user.Roles = new List<UserRole> { adminUserRole };
            adminUserRole.Users = new List<User> { user };
            var roleService = new RoleService(session);
            roleService.SaveRole(adminUserRole);

            user.Sites = new List<Site> { site };
            site.Users = new List<User> { user };
            siteService.SaveSite(site);

            authorisationService.Logout();
            authorisationService.SetAuthCookie(user.Email, false);

            //set up system tasks
            //var taskService = MrCMSApplication.Get<IScheduledTaskManager>();
            //taskService.Add(new ScheduledTask
            //                    {
            //                        Type = "MrCMS.Tasks.SendQueuedMessagesTask",
            //                        EveryXMinutes = 1,
            //                        Site = site
            //                    });
        }

        public virtual void RestartAppDomain()
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

            // If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            if (_context != null)
            {
                _context.Response.Redirect("~", true);
            }
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/web.config"), DateTime.UtcNow);
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
        public virtual string MapPath(string path)
        {
            if (_context != null)
                return HostingEnvironment.MapPath(path);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int binIndex = baseDirectory.IndexOf("\\bin\\", StringComparison.Ordinal);
            if (binIndex >= 0)
                baseDirectory = baseDirectory.Substring(0, binIndex);
            else if (baseDirectory.EndsWith("\\bin"))
                baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 4);

            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }


        /// <summary>
        ///     Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        private static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
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
            }
            return _trustLevel.Value;
        }


        /// <summary>
        ///     Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Returns true if the database exists.</returns>
        private bool sqlServerDatabaseExists(string connectionString)
        {
            try
            {
                //just try to connect
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        ///     Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Returns true if the database exists.</returns>
        private bool mySqlServerDatabaseExists(string connectionString)
        {
            try
            {
                //just try to connect
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Creates a database on the server.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Error</returns>
        private string createSqlDatabase(string connectionString)
        {
            try
            {
                //parse database name
                var builder = new SqlConnectionStringBuilder(connectionString);
                string databaseName = builder.InitialCatalog;
                //now create connection string to 'master' dabatase. It always exists.
                builder.InitialCatalog = "master";
                string masterCatalogConnectionString = builder.ToString();
                string query = string.Format("CREATE DATABASE [{0}] COLLATE SQL_Latin1_General_CP1_CI_AS", databaseName);

                using (var conn = new SqlConnection(masterCatalogConnectionString))
                {
                    conn.Open();
                    using (var command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("An error occured when creating database: {0}", ex.Message);
            }
        }

        /// <summary>
        ///     Creates a database on the server.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Error</returns>
        private string createMySqlDatabase(string connectionString)
        {
            try
            {
                //parse database name
                var builder = new MySqlConnectionStringBuilder(connectionString);
                string databaseName = builder.Database;
                //now create connection string to 'master' dabatase. It always exists.
                builder.Database = "";
                string masterCatalogConnectionString = builder.ToString();
                string query = string.Format("CREATE DATABASE {0}", databaseName);

                using (var conn = new MySqlConnection(masterCatalogConnectionString))
                {
                    conn.Open();
                    using (var command = new MySqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("An error occured when creating database: {0}", ex.Message);
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
        private bool checkPermissions(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            AuthorizationRuleCollection rules = null;
            try
            {
                rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier));
            }
            catch
            {
                return true;
            }
            try
            {
                foreach (
                    FileSystemAccessRule rule in
                        rules.Cast<FileSystemAccessRule>().Where(rule => current.User.Equals(rule.IdentityReference)))
                {
                    if (AccessControlType.Deny.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                            flag4 = true;
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                            flag3 = true;

                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                            flag = true;

                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                            flag2 = true;

                        continue;
                    }
                    if (AccessControlType.Allow.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                        {
                            flag8 = true;
                        }
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                        {
                            flag7 = true;
                        }
                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                        {
                            flag5 = true;
                        }
                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                        {
                            flag6 = true;
                        }
                    }
                }
                foreach (FileSystemAccessRule rule2 in
                    from reference in current.Groups
                    from FileSystemAccessRule rule2 in rules
                    where reference.Equals(rule2.IdentityReference)
                    select rule2)
                {
                    if (AccessControlType.Deny.Equals(rule2.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                            flag4 = true;
                        if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                            flag3 = true;
                        if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                            flag = true;
                        if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                            flag2 = true;
                        continue;
                    }
                    if (AccessControlType.Allow.Equals(rule2.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                            flag8 = true;
                        if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                            flag7 = true;
                        if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                            flag5 = true;
                        if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                            flag6 = true;
                    }
                }
                bool flag9 = !flag4 && flag8;
                bool flag10 = !flag3 && flag7;
                bool flag11 = !flag && flag5;
                bool flag12 = !flag2 && flag6;
                bool flag13 = true;
                if (checkRead)
                {
                    flag13 = flag13 && flag11;
                }
                if (checkWrite)
                {
                    flag13 = flag13 && flag12;
                }
                if (checkModify)
                {
                    flag13 = flag13 && flag10;
                }
                if (checkDelete)
                {
                    flag13 = flag13 && flag9;
                }
                return flag13;
            }
            catch (IOException)
            {
            }
            return false;
        }

        /// <summary>
        ///     Create contents of connection strings used by the SqlConnection class
        /// </summary>
        /// <param name="trustedConnection">Avalue that indicates whether User ID and Password are specified in the connection (when false) or whether the current Windows account credentials are used for authentication (when true)</param>
        /// <param name="serverName">The name or network address of the instance of SQL Server to connect to</param>
        /// <param name="databaseName">The name of the database associated with the connection</param>
        /// <param name="userName">The user ID to be used when connecting to SQL Server</param>
        /// <param name="password">The password for the SQL Server account</param>
        /// <param name="timeout">The connection timeout</param>
        /// <returns>Connection string</returns>
        private string createConnectionString(bool trustedConnection,
                                              string serverName, string databaseName, string userName, string password,
                                              int timeout = 0)
        {
            var builder = new SqlConnectionStringBuilder();
            builder.IntegratedSecurity = trustedConnection;
            builder.DataSource = serverName;
            builder.InitialCatalog = databaseName;
            if (!trustedConnection)
            {
                builder.UserID = userName;
                builder.Password = password;
            }
            builder.PersistSecurityInfo = false;
            builder.MultipleActiveResultSets = true;
            if (timeout > 0)
            {
                builder.ConnectTimeout = timeout;
            }
            return builder.ConnectionString;
        }

        /// <summary>
        ///     Create contents of connection strings used by the SqlConnection class
        /// </summary>
        /// <param name="trustedConnection">Avalue that indicates whether User ID and Password are specified in the connection (when false) or whether the current Windows account credentials are used for authentication (when true)</param>
        /// <param name="serverName">The name or network address of the instance of SQL Server to connect to</param>
        /// <param name="databaseName">The name of the database associated with the connection</param>
        /// <param name="userName">The user ID to be used when connecting to SQL Server</param>
        /// <param name="password">The password for the SQL Server account</param>
        /// <param name="timeout">The connection timeout</param>
        /// <returns>Connection string</returns>
        private string createMySqlConnectionString(bool trustedConnection,
                                                   string serverName, string databaseName, string userName, string password,
                                                   int timeout = 0)
        {
            var builder = new MySqlConnectionStringBuilder();
            builder.IntegratedSecurity = trustedConnection;
            builder.Server= serverName;
            builder.Database= databaseName;
            if (!trustedConnection)
            {
                builder.UserID = userName;
                builder.Password = password;
            }
            builder.PersistSecurityInfo = false;
            if (timeout > 0)
            {
                builder.ConnectionTimeout = Convert.ToUInt32(timeout);
            }
            return builder.ConnectionString;
        }
    }
}