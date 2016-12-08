using MrCMS.Installation;

namespace MrCMS.DbConfiguration.Helpers
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