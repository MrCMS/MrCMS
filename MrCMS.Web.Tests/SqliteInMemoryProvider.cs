using FluentNHibernate.Cfg.Db;
using MrCMS.DbConfiguration;
using MrCMS.Installation;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace MrCMS.Web.Tests
{
    public class SqliteInMemoryProvider : IDatabaseProvider
    {
        public string DisplayName { get; private set; }
        public string Type { get; private set; }
        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return SQLiteConfiguration.Standard.Dialect<SQLiteDialect>()
                .InMemory()
                .Raw(Environment.ReleaseConnections, "on_close");
        }

        public string GetConnectionString(InstallModel model)
        {
            return null;
        }

        public void CreateDatabase(InstallModel model)
        {
        }

        public bool RequiresConnectionStringBuilding { get; private set; }
    }
}