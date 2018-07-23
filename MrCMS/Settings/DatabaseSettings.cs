namespace MrCMS.Settings
{
    public class DatabaseSettings : SystemSettingsBase
    {
        [AppSettingName("mrcms-database-provider")]
        public string DatabaseProviderType { get; set; }

        [ConnectionString("mrcms")]
        public string ConnectionString { get; set; }
    }
}