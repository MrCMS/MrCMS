using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    //public class CreateSqlServer2008Database : CreateSqlServerDatabase, ICreateDatabase<SqlServer2008Provider>
    //{
    //    protected override IDatabaseProvider GetProvider(string connectionString)
    //    {
    //        return new SqlServer2008Provider(new OptionsWrapper<DatabaseSettings>(new DatabaseSettings
    //        {
    //            ConnectionString = connectionString,
    //            DatabaseProviderType = typeof(CreateSqlServer2008Database).FullName
    //        }));
    //    }
    //}
}