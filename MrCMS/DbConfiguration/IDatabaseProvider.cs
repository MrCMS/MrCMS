using FluentNHibernate.Cfg.Db;
using MrCMS.Installation;

namespace MrCMS.DbConfiguration
{
    public interface IDatabaseProvider
    {
        string Type { get; }
        IPersistenceConfigurer GetPersistenceConfigurer();
    }
}