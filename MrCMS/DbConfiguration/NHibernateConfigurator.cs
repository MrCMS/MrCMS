using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlServerCe;
using System.Reflection;
using System.Web;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.DbConfiguration.Conventions;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.DbConfiguration.Overrides;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Caches.SysCache2;
using NHibernate.Dialect;
using NHibernate.Event;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.DbConfiguration
{
    public class NHibernateConfigurator
    {
        private List<Assembly> _manuallyAddedAssemblies = new List<Assembly>();
        public DatabaseType DatabaseType { get; set; }
        public bool InDevelopment { get; set; }
        public bool CacheEnabled { get; set; }

        public IPersistenceConfigurer PersistenceOverride { get; set; }

        public ISessionFactory CreateSessionFactory()
        {
            var configuration = GetConfiguration();

            return configuration.BuildSessionFactory();
        }

        private IPersistenceConfigurer GetPersistenceConfigurer()
        {
            if (PersistenceOverride != null)
                return PersistenceOverride;
            switch (DatabaseType)
            {
                case DatabaseType.Auto:
                    var connectionStringSettings = ConfigurationManager.ConnectionStrings["mrcms"];
                    if (connectionStringSettings != null && "System.Data.SQLite".Equals(connectionStringSettings.ProviderName,StringComparison.OrdinalIgnoreCase))
                        return InDevelopment
                                   ? SQLiteConfiguration.Standard.ConnectionString(
                                       x => x.FromConnectionStringWithKey("mrcms-dev"))
                                   : SQLiteConfiguration.Standard.ConnectionString(
                                       x => x.FromConnectionStringWithKey("mrcms"));
                    return InDevelopment
                               ? MsSqlConfiguration.MsSql2008.ConnectionString(
                                   x => x.FromConnectionStringWithKey("mrcms-dev"))
                               : MsSqlConfiguration.MsSql2008.ConnectionString(
                                   x => x.FromConnectionStringWithKey("mrcms"));
                case DatabaseType.MsSql:
                    return InDevelopment
                               ? MsSqlConfiguration.MsSql2008.ConnectionString(
                                   x => x.FromConnectionStringWithKey("mrcms-dev"))
                               : MsSqlConfiguration.MsSql2008.ConnectionString(
                                   x => x.FromConnectionStringWithKey("mrcms"));
                case DatabaseType.Sqlite:
                    return SQLiteConfiguration.Standard.Dialect<SQLiteDialect>().InMemory().Raw(
                        Environment.ReleaseConnections, "on_close");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void ValidateSchema(NHibernate.Cfg.Configuration config)
        {
            var validator = new SchemaValidator(config);
            try
            {
                validator.Validate();
            }
            catch (HibernateException)
            {
                var update = new SchemaUpdate(config);
                update.Execute(false, true);
            }
        }

        public NHibernate.Cfg.Configuration GetConfiguration()
        {
            var assemblies1 = AppDomain.CurrentDomain.GetAssemblies();
            var assemblies =
                assemblies1.Where(
                    assembly => !assembly.IsDynamic && !assembly.GlobalAssemblyCache).ToList();
            assemblies.AddRange(ManuallyAddedAssemblies);
            assemblies = assemblies.Distinct().ToList();

            var config = Fluently.Configure()
                .Database(GetPersistenceConfigurer())
                .Mappings(m => m.AutoMappings.Add(AutoMap.Assemblies(new TheventsMappingConfiguration(), assemblies)
                                                .IgnoreBase<BaseEntity>().IgnoreBase<BaseDocumentItemEntity>()
                                                .IncludeBase<Document>().IncludeBase<Webpage>().IncludeBase<Widget>().IncludeBase<Layout>()
                                                .UseOverridesFromAssemblies(assemblies.Where(assembly => !assembly.GlobalAssemblyCache).ToArray())
                                                .Conventions.AddFromAssemblyOf<CustomForeignKeyConvention>()))
                .Cache(builder =>
                           {
                               if (CacheEnabled)
                                   builder.UseSecondLevelCache().UseQueryCache().ProviderClass<SysCacheProvider>().
                                       QueryCacheFactory<StandardQueryCacheFactory>();
                           })
                .ExposeConfiguration(AppendListeners)
                .ExposeConfiguration(c =>
                                         {
                                             c.SetProperty(Environment.GenerateStatistics, "true");
                                             c.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
                                         })
                .BuildConfiguration();

            ValidateSchema(config);

            config.BuildMappings();


            return config;
        }

        public List<Assembly> ManuallyAddedAssemblies { get { return _manuallyAddedAssemblies; } set { _manuallyAddedAssemblies = value; } }

        private void AppendListeners(NHibernate.Cfg.Configuration configuration)
        {
            var saveOrUpdateListener = new SaveOrUpdateListener();
            configuration.EventListeners.SaveOrUpdateEventListeners =
                 new ISaveOrUpdateEventListener[]
                      {
                          saveOrUpdateListener
                      };

            configuration.AppendListeners(ListenerType.PreInsert,
                                          new[]
                                              {
                                                  saveOrUpdateListener
                                              });
            configuration.AppendListeners(ListenerType.PreUpdate,
                                          new[]
                                              {
                                                  saveOrUpdateListener
                                              });

            configuration.AppendListeners(ListenerType.PostCommitUpdate, new[]
                                                                             {
                                                                                 new PostCommitEventListener()
                                                                             });
        }
    }
}