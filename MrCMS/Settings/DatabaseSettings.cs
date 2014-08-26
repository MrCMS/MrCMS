namespace MrCMS.Settings
{
    public class DatabaseSettings : SystemSettingsBase
    {
        public string DatabaseProviderType { get; set; }
        public string ConnectionString { get; set; }
        public string DbPrefix { get; set; }
    }
}