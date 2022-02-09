namespace MrCMS.DbConfiguration
{
    public interface IDatabaseProviderResolver
    {
        IDatabaseProvider GetProvider();
        bool IsProviderConfigured();
    }
}