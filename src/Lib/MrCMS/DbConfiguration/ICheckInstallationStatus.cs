namespace MrCMS.DbConfiguration
{
    public interface ICheckInstallationStatus
    {
        InstallationStatus GetStatus();
        bool IsInstalled();
    }
}