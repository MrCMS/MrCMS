namespace MrCMS.Installation
{
    public interface IInstallationService
    {
        InstallationResult Install(InstallModel model);
    }
}