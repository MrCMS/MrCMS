using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    public class CreateSqlServer2008Database : CreateSqlServerDatabase, ICreateDatabase<SqlServer2008Provider>
    {
        protected override IDatabaseProvider GetProvider(string connectionString)
        {
            return new SqlServer2008Provider(new DatabaseSettings
            {
                ConnectionString = connectionString
            });
        }
    }
}