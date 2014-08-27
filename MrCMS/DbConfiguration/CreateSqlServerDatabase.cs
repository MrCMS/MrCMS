using System;
using System.Data.SqlClient;
using System.Threading;
using MrCMS.Installation;

namespace MrCMS.DbConfiguration
{
    public class CreateSqlServerDatabase : ICreateDatabase<SqlServer2008Provider>,
        ICreateDatabase<SqlServer2012Provider>
    {
        public void CreateDatabase(InstallModel model)
        {
            string connectionString = GetConnectionString(model);

            if (model.SqlServerCreateDatabase)
            {
                if (!SqlServerDatabaseExists(connectionString))
                {
                    //create database
                    string errorCreatingDatabase = CreateSqlDatabase(connectionString);
                    if (!String.IsNullOrEmpty(errorCreatingDatabase))
                        throw new Exception(errorCreatingDatabase);
                    //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
                    //that's just wait 3 seconds
                    Thread.Sleep(3000);
                }
            }
            else
            {
                //check whether database exists
                if (!SqlServerDatabaseExists(connectionString))
                    throw new Exception("Database does not exist or you don't have permissions to connect to it");
            }
        }

        public string GetConnectionString(InstallModel model)
        {
            if (model.SqlConnectionInfo == SqlConnectionInfo.Raw)
            {
                //raw connection string
                return model.DatabaseConnectionString;
            }
            return CreateConnectionString(model);
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
        ///     Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Returns true if the database exists.</returns>
        private bool SqlServerDatabaseExists(string connectionString)
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

        private string CreateConnectionString(InstallModel model)
        {
            var builder = new SqlConnectionStringBuilder();
            bool trustedConnection = model.SqlAuthenticationType == SqlAuthenticationType.Windows;
            builder.IntegratedSecurity = trustedConnection;
            builder.DataSource = model.SqlServerName;
            builder.InitialCatalog = model.SqlDatabaseName;
            if (!trustedConnection)
            {
                builder.UserID = model.SqlServerUsername;
                builder.Password = model.SqlServerPassword;
            }
            builder.PersistSecurityInfo = false;
            builder.MultipleActiveResultSets = true;
            return builder.ConnectionString;
        }
    }
}