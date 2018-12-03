using Microsoft.AspNetCore.Hosting;
using MrCMS.Installation;
using System.IO;

namespace MrCMS.DbConfiguration
{
    //public class CreateSQLiteDatabase : ICreateDatabase<SqliteProvider>
    //{
    //    private readonly IHostingEnvironment _hostingEnvironment;

    //    public CreateSQLiteDatabase(IHostingEnvironment hostingEnvironment)
    //    {
    //        _hostingEnvironment = hostingEnvironment;
    //    }
    //    private const string DatabaseFileName = "MrCMS.Db.db";
    //    private const string DatabasePath = @"|DataDirectory|\" + DatabaseFileName;
    //    private const string ConnectionString = "Data Source=" + DatabasePath;

    //    public void CreateDatabase(InstallModel model)
    //    {  //drop database if exists
    //        string databaseFullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "/App_Data/", DatabaseFileName);
    //        if (File.Exists(databaseFullPath))
    //        {
    //            File.Delete(databaseFullPath);
    //        }
    //        using (File.Create(databaseFullPath))
    //        {
    //        }
    //    }

    //    public string GetConnectionString(InstallModel model)
    //    {
    //        return ConnectionString;
    //    }
    //}
}