using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    public class CreateSqlServerAzureDatabase :CreateSqlServerDatabase, ICreateDatabase<SqlServerAzureProvider>
    {
        protected override IDatabaseProvider GetProvider(string connectionString)
        {
            return new SqlServerAzureProvider(new DatabaseSettings
            {
                ConnectionString = connectionString
            });
        }
    }
}