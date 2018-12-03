using MrCMS.Installation;

namespace MrCMS.DbConfiguration
{
    public interface ICreateDatabase
    {
        IDatabaseProvider CreateDatabase(InstallModel model);
        string GetConnectionString(InstallModel model);
    }

    public interface ICreateDatabase<T> : ICreateDatabase
    {
    }
}