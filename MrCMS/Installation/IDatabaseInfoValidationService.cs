using System;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MrCMS.Installation
{
    public interface IDatabaseInfoValidationService
    {
        InstallationResult ValidateDbInfo(InstallModel model);
    }

    public class DatabaseInfoValidationService : IDatabaseInfoValidationService
    {
        public InstallationResult ValidateDbInfo(InstallModel model)
        {
            var result = new InstallationResult();
            ////SQL Server
            //switch (model.DatabaseType)
            //{
            //    case DatabaseType.Sqlite:
            //        break;
            //    case DatabaseType.MsSql:
            //        if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw",
            //            StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            //raw connection string
            //            if (string.IsNullOrEmpty(model.DatabaseConnectionString))
            //                result.AddModelError("A SQL connection string is required");

            //            try
            //            {
            //                //try to create connection string
            //                new SqlConnectionStringBuilder(model.DatabaseConnectionString);
            //            }
            //            catch
            //            {
            //                result.AddModelError("Wrong SQL connection string format");
            //            }
            //        }
            //        else
            //        {
            //            //values
            //            if (string.IsNullOrEmpty(model.SqlServerName))
            //                result.AddModelError("SQL Server name is required");
            //            if (string.IsNullOrEmpty(model.SqlDatabaseName))
            //                result.AddModelError("Database name is required");

            //            //authentication type
            //            if (model.SqlAuthenticationType.Equals("sqlauthentication",
            //                StringComparison.InvariantCultureIgnoreCase))
            //            {
            //                //SQL authentication
            //                if (string.IsNullOrEmpty(model.SqlServerUsername))
            //                    result.AddModelError("SQL Username is required");
            //                if (string.IsNullOrEmpty(model.SqlServerPassword))
            //                    result.AddModelError("SQL Password is required");
            //            }
            //        }
            //        break;
            //    case DatabaseType.MySQL:
            //        if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw",
            //            StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            //raw connection string
            //            if (string.IsNullOrEmpty(model.DatabaseConnectionString))
            //                result.AddModelError("A SQL connection string is required");

            //            try
            //            {
            //                //try to create connection string
            //                new MySqlConnectionStringBuilder(model.DatabaseConnectionString);
            //            }
            //            catch
            //            {
            //                result.AddModelError("Wrong SQL connection string format");
            //            }
            //        }
            //        else
            //        {
            //            //values
            //            if (string.IsNullOrEmpty(model.SqlServerName))
            //                result.AddModelError("SQL Server name is required");
            //            if (string.IsNullOrEmpty(model.SqlDatabaseName))
            //                result.AddModelError("Database name is required");

            //            //authentication type
            //            if (model.SqlAuthenticationType.Equals("sqlauthentication",
            //                StringComparison.InvariantCultureIgnoreCase))
            //            {
            //                //SQL authentication
            //                if (string.IsNullOrEmpty(model.SqlServerUsername))
            //                    result.AddModelError("SQL Username is required");
            //                if (string.IsNullOrEmpty(model.SqlServerPassword))
            //                    result.AddModelError("SQL Password is required");
            //            }
            //        }
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
            return result;
        }
    }
}