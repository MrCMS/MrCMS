namespace MrCMS.DbConfiguration
{
    public enum InstallationStatus
    {
        RequiresDatabaseSettings,
        RequiresMigrations,
        RequiresInstallation,
        Installed
    }
}