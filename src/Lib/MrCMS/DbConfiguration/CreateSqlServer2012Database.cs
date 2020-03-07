using Microsoft.Extensions.Options;
using MrCMS.Installation.Models;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    //public class CreateSqlServer2012Database : CreateSqlServerDatabase, ICreateDatabase<SqlServer2012Provider>
    //{
    //    protected override IDatabaseProvider GetProvider(string connectionString)
    //    {
    //        return new SqlServer2012Provider(new OptionsWrapper<DatabaseSettings>(new DatabaseSettings
    //        {
    //            ConnectionString = connectionString,
    //            DatabaseProviderType = typeof(CreateSqlServer2012Database).FullName
    //        }));
    //    }
    //}

    ////public class CreateSqlServerAzureDatabase : CreateSqlServerDatabase, ICreateDatabase<SqlServerAzureProvider>
    ////{
    ////    protected override IDatabaseProvider GetProvider(string connectionString)
    ////    {
    ////        return new SqlServerAzureProvider(new DatabaseSettings
    ////        {
    ////            ConnectionString = connectionString
    ////        });
    ////    }
    ////}
}