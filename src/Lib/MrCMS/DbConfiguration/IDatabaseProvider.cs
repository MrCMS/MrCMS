using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg.Loquacious;

namespace MrCMS.DbConfiguration
{
    public interface IDatabaseProvider
    {
        string Type { get; }
        IPersistenceConfigurer GetPersistenceConfigurer();
        void AddProviderSpecificConfiguration(NHibernate.Cfg.Configuration config);
        void DebugDatabaseIntegration(DbIntegrationConfigurationProperties properties);
    }
}