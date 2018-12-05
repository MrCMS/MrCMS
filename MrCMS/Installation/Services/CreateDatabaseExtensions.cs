using MrCMS.DbConfiguration;
using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public static class CreateDatabaseExtensions
    {
        public static bool ValidateConnectionString(this ICreateDatabase createDatabase, InstallModel installModel)
        {
            try
            {
                createDatabase.GetConnectionString(installModel);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}