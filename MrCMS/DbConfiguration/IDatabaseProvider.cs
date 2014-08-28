using FluentNHibernate.Cfg.Db;

namespace MrCMS.DbConfiguration
{
    public interface IDatabaseProvider
    {
        string Type { get; }
        IPersistenceConfigurer GetPersistenceConfigurer();
    }
}