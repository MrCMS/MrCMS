using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Settings;
using MySql.Data.MySqlClient;
using Ninject;

namespace MrCMS.Installation
{
    public interface IDatabaseCreationService
    {
        IDatabaseProvider CreateDatabase(InstallModel model);
    }

    public class DatabaseCreationService : IDatabaseCreationService
    {
        private readonly IKernel _kernel;
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;

        public DatabaseCreationService(IKernel kernel,
            ISystemConfigurationProvider systemConfigurationProvider)
        {
            
            _kernel = kernel;
            _systemConfigurationProvider = systemConfigurationProvider;
        }

        public IDatabaseProvider CreateDatabase(InstallModel model)
        {

            var creatorType =
                TypeHelper.GetAllConcreteTypesAssignableFrom(
                    typeof (ICreateDatabase<>).MakeGenericType(TypeHelper.GetTypeByName(model.DatabaseProvider))).FirstOrDefault();
            if (creatorType == null)
                return null;
            var createDatabase = _kernel.Get(creatorType) as ICreateDatabase;
            createDatabase.CreateDatabase(model);
            SaveConnectionSettings(createDatabase, model);
            return _kernel.GetAll<IDatabaseProvider>().FirstOrDefault(provider => provider.Type == model.DatabaseProvider);

            //string connectionString = null;
            //switch (model.DatabaseType)
            //{
            //    case DatabaseType.Sqlite:
            //        //SQLite
            //        string databaseFileName = "MrCMS.Db.db";
            //        string databasePath = @"|DataDirectory|\" + databaseFileName;
            //        connectionString = "Data Source=" + databasePath;

            //        //drop database if exists
            //        string databaseFullPath = HostingEnvironment.MapPath("~/App_Data/") + databaseFileName;
            //        if (File.Exists(databaseFullPath))
            //        {
            //            File.Delete(databaseFullPath);
            //        }
            //        using (File.Create(databaseFullPath))
            //        {
            //        }
            //        break;
            //    case DatabaseType.MsSql:
            //        if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw",
            //            StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            //raw connection string
            //            connectionString = model.DatabaseConnectionString;
            //        }
            //        else
            //        {
            //            //values
            //            connectionString =
            //                createConnectionString(model.SqlAuthenticationType == "windowsauthentication",
            //                    model.SqlServerName, model.SqlDatabaseName,
            //                    model.SqlServerUsername, model.SqlServerPassword);
            //        }

            //        if (model.SqlServerCreateDatabase)
            //        {
            //            if (!sqlServerDatabaseExists(connectionString))
            //            {
            //                //create database
            //                string errorCreatingDatabase = CreateSqlDatabase(connectionString);
            //                if (!String.IsNullOrEmpty(errorCreatingDatabase))
            //                    throw new Exception(errorCreatingDatabase);
            //                //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
            //                //that's just wait 3 seconds
            //                Thread.Sleep(3000);
            //            }
            //        }
            //        else
            //        {
            //            //check whether database exists
            //            if (!sqlServerDatabaseExists(connectionString))
            //                throw new Exception(
            //                    "Database does not exist or you don't have permissions to connect to it");
            //        }
            //        break;
            //    case DatabaseType.MySQL:
            //        if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw",
            //            StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            //raw connection string
            //            connectionString = model.DatabaseConnectionString;
            //        }
            //        else
            //        {
            //            //values
            //            connectionString =
            //                createMySqlConnectionString(model.SqlAuthenticationType == "windowsauthentication",
            //                    model.SqlServerName, model.SqlDatabaseName,
            //                    model.SqlServerUsername, model.SqlServerPassword);
            //        }

            //        if (model.SqlServerCreateDatabase)
            //        {
            //            if (!mySqlServerDatabaseExists(connectionString))
            //            {
            //                //create database
            //                string errorCreatingDatabase = createMySqlDatabase(connectionString);
            //                if (!String.IsNullOrEmpty(errorCreatingDatabase))
            //                    throw new Exception(errorCreatingDatabase);
            //                //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
            //                //that's just wait 3 seconds
            //                Thread.Sleep(3000);
            //            }
            //        }
            //        else
            //        {
            //            //check whether database exists
            //            if (!mySqlServerDatabaseExists(connectionString))
            //                throw new Exception(
            //                    "Database does not exist or you don't have permissions to connect to it");
            //        }
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
            //return connectionString;
        }

        public void SaveConnectionSettings(ICreateDatabase provider, InstallModel installModel)
        {
            var databaseSettings = _systemConfigurationProvider.GetSystemSettings<DatabaseSettings>();

            databaseSettings.ConnectionString = provider.GetConnectionString(installModel);
            databaseSettings.DatabaseProviderType = installModel.DatabaseProvider;

            _systemConfigurationProvider.SaveSettings(databaseSettings);
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
        private string CreateSqlDatabase(string connectionString)
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
        ///     Create contents of connection strings used by the SqlConnection class
        /// </summary>
        /// <param name="trustedConnection">
        ///     Avalue that indicates whether User ID and Password are specified in the connection
        ///     (when false) or whether the current Windows account credentials are used for authentication (when true)
        /// </param>
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
            builder.Server = serverName;
            builder.Database = databaseName;
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