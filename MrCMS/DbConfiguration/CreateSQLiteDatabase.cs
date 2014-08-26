using System.IO;
using System.Web.Hosting;
using MrCMS.Installation;

namespace MrCMS.DbConfiguration
{
    public class CreateSQLiteDatabase : ICreateDatabase<SqliteProvider>
    {
        private const string DatabaseFileName = "MrCMS.Db.db";
        private const string DatabasePath = @"|DataDirectory|\" + DatabaseFileName;
        private const string ConnectionString = "Data Source=" + DatabasePath;

        public void CreateDatabase(InstallModel model)
        {  //drop database if exists
            string databaseFullPath = HostingEnvironment.MapPath("~/App_Data/") + DatabaseFileName;
            if (File.Exists(databaseFullPath))
            {
                File.Delete(databaseFullPath);
            }
            using (File.Create(databaseFullPath))
            {
            }
        }

        public string GetConnectionString(InstallModel model)
        {
            return ConnectionString;
        }
    }
}