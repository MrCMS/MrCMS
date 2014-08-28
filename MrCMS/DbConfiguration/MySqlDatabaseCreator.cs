using MrCMS.Installation;
using MySql.Data.MySqlClient;

namespace MrCMS.DbConfiguration
{
    public class MySqlDatabaseCreator : ICreateDatabase<MySqlProvider>
    {
        public void CreateDatabase(InstallModel model)
        {
            //parse database name
            var builder = new MySqlConnectionStringBuilder(GetConnectionString(model));
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
        }

        public string GetConnectionString(InstallModel model)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                IntegratedSecurity = model.SqlAuthenticationType == SqlAuthenticationType.Windows,
                Server = model.SqlServerName,
                Database = model.SqlDatabaseName,
                PersistSecurityInfo = false
            };
            if (!builder.IntegratedSecurity)
            {
                builder.UserID = model.SqlServerUsername;
                builder.Password = model.SqlServerPassword;
            }
            return builder.ConnectionString;
        }
    }
}