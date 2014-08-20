using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using FluentNHibernate.Cfg.Db;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Installation
{
    public class InstallationService : IInstallationService
    {
        private static AspNetHostingPermissionLevel? _trustLevel;
        private readonly IDatabaseCreationService _databaseCreationService;
        private readonly IDatabaseInfoValidationService _databaseInfoValidationService;
        private readonly IFileSystemAccessService _fileSystemAccessService;

        public InstallationService(IDatabaseInfoValidationService databaseInfoValidationService,
            IFileSystemAccessService fileSystemAccessService,
            IDatabaseCreationService databaseCreationService)
        {
            _databaseInfoValidationService = databaseInfoValidationService;
            _fileSystemAccessService = fileSystemAccessService;
            _databaseCreationService = databaseCreationService;
        }

        public InstallationResult Install(InstallModel model)
        {
            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            InstallationResult result = _databaseInfoValidationService.ValidateDbInfo(model);
            if (!result.Success)
                return result;

            result = _fileSystemAccessService.EnsureAccessToFileSystem();
            if (!result.Success)
                return result;

            try
            {
                string connectionString = _databaseCreationService.CreateDatabase(model);

                //save settings
                SetUpInitialData(model, connectionString, model.DatabaseType);

                CurrentRequestData.OnEndRequest.Add(k =>
                {
                    var connectionStringSettings =
                        new ConnectionStringSettings("mrcms",
                            connectionString)
                        {
                            ProviderName =
                                GetProviderName(model.DatabaseType)
                        };
                    Configuration cfg =
                        WebConfigurationManager.OpenWebConfiguration(@"/");
                    cfg.ConnectionStrings.ConnectionStrings.Remove("mrcms");
                    cfg.ConnectionStrings.ConnectionStrings.Add(
                        connectionStringSettings);
                    cfg.Save();

                    RestartAppDomain();
                });
            }
            catch (Exception exception)
            {
                result.AddModelError("Setup failed: " + exception);
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
            ISession session = sessionFactory.OpenFilteredSession();
            var kernel = MrCMSApplication.Get<IKernel>();
            kernel.Rebind<ISession>().ToMethod(context => session);
            var site = new Site
            {
                Name = model.SiteName,
                BaseUrl = model.SiteUrl,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            using (IStatelessSession statelessSession = sessionFactory.OpenStatelessSession())
            {
                using (ITransaction transaction = statelessSession.BeginTransaction())
                {
                    statelessSession.Insert(site);
                    transaction.Commit();
                }
            }
            CurrentRequestData.CurrentSite = site;

            kernel.Get<IInitializeDatabase>().Initialize(model);
            kernel.Get<ICreateInitialUser>().Create(model);
            kernel.GetAll<IOnInstallation>()
                .OrderBy(installation => installation.Priority)
                .ForEach(installation => installation.Install(model));

            SetupInitialTemplates(session);
        }

        private static void SetupInitialTemplates(ISession session)
        {
            session.Transact(s =>
            {
                foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<MessageTemplate>())
                {
                    if (s.CreateCriteria(type).List().Count == 0)
                    {
                        var messageTemplate = Activator.CreateInstance(type) as MessageTemplate;
                        if (messageTemplate != null && messageTemplate.GetInitialTemplate(s) != null)
                            s.Save(messageTemplate.GetInitialTemplate(s));
                    }
                }
            });
        }

        public static void RestartAppDomain()
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

        private static bool TryWriteWebConfig()
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
        public static string MapPath(string path)
        {
            if (HttpContext.Current != null)
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
        ///     Finds the trust level of the running application
        ///     (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
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
    }
}