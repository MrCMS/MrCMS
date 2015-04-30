using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    public class CreateSqlServer2012Database : CreateSqlServerDatabase, ICreateDatabase<SqlServer2012Provider>
    {
        protected override IDatabaseProvider GetProvider(string connectionString)
        {
            return new SqlServer2012Provider(new DatabaseSettings
            {
                ConnectionString = connectionString
            });
        }
    }
}