using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MrCMS.DbConfiguration;
using MrCMS.Installation.Models;
using MrCMS.Settings;

namespace MrCMS.Data.Sqlite
{
    public class CreateSQLiteDatabase : ICreateDatabase<SqliteProvider>
    {
        private const string DatabaseFileName = "MrCMS.Db.db";
        //private const string DatabasePath = @"|DataDirectory|\" + DatabaseFileName;
        //private const string ConnectionString = "Data Source=" + DatabasePath;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CreateSQLiteDatabase(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IDatabaseProvider CreateDatabase(InstallModel model)
        {
            //drop database if exists
            var databaseFullPath = GetDatabaseFullPath();
            if (File.Exists(databaseFullPath)) File.Delete(databaseFullPath);
            using (File.Create(databaseFullPath))
            {
            }

            return new SqliteProvider(new OptionsWrapper<DatabaseSettings>(new DatabaseSettings
            {
                DatabaseProviderType = typeof(SqliteProvider).FullName,
                ConnectionString = GetConnectionString()
            }));
        }

        private string GetConnectionString()
        {
            return $"Data Source={GetDatabasePath()}";
        }

        private string GetDatabaseFullPath()
        {
            return Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", DatabaseFileName);
        }
        private string GetDatabasePath()
        {
            return Path.Combine("App_Data", DatabaseFileName);
        }

        public string GetConnectionString(InstallModel model)
        {
            return GetConnectionString();
        }
    }
}