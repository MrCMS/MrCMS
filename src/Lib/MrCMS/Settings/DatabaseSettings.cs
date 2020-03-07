namespace MrCMS.Settings
{
    public class DatabaseSettings 
    {
        public string DatabaseProviderType { get; set; }

        public string ConnectionString { get; set; }

        public bool AreSet()
        {
            return !string.IsNullOrWhiteSpace(DatabaseProviderType) && !string.IsNullOrWhiteSpace(ConnectionString);
        }
    }
}