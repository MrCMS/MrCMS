using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.DbConfiguration.Conventions;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.Widget;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Caches.SysCache2;
using NHibernate.Dialect;
using NHibernate.Event;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;
using System.Linq;
using MrCMS.Helpers;

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
                    switch (connectionStringSettings.ProviderName)
                    {
                        case "System.Data.SQLite":
                            return InDevelopment
                                       ? SQLiteConfiguration.Standard.ConnectionString(
                                           x => x.FromConnectionStringWithKey("mrcms-dev"))
                                       : SQLiteConfiguration.Standard.ConnectionString(
                                           x => x.FromConnectionStringWithKey("mrcms"));
                        case "System.Data.SqlClient":
                            return InDevelopment
                         ? MsSqlConfiguration.MsSql2008.ConnectionString(
                             x => x.FromConnectionStringWithKey("mrcms-dev"))
                         : MsSqlConfiguration.MsSql2008.ConnectionString(
                             x => x.FromConnectionStringWithKey("mrcms"));
                        case "MySql.Data.MySqlClient":
                            return InDevelopment
                                       ? MySQLConfiguration.Standard.ConnectionString(
                                           x => x.FromConnectionStringWithKey("mrcms-dev"))
                                       : MySQLConfiguration.Standard.ConnectionString(
                                           x => x.FromConnectionStringWithKey("mrcms"));
                    }
                    throw new DataException("Provider Name not recognised: " + connectionStringSettings.ProviderName);
                case DatabaseType.MsSql:
                    return InDevelopment
                               ? MsSqlConfiguration.MsSql2008.ConnectionString(
                                   x => x.FromConnectionStringWithKey("mrcms-dev"))
                               : MsSqlConfiguration.MsSql2008.ConnectionString(
                                   x => x.FromConnectionStringWithKey("mrcms"));
                case DatabaseType.MySQL:
                    return InDevelopment
                               ? MySQLConfiguration.Standard.ConnectionString(
                                   x => x.FromConnectionStringWithKey("mrcms-dev"))
                               : MySQLConfiguration.Standard.ConnectionString(
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
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblies =
                currentAssemblies.Where(
                    assembly =>
                    !assembly.IsDynamic && !assembly.GlobalAssemblyCache &&
                    !assembly.FullName.Contains("xunit", StringComparison.OrdinalIgnoreCase)).ToList();
            assemblies.AddRange(ManuallyAddedAssemblies);

            var finalAssemblies = new List<Assembly>();

            assemblies.ForEach(assembly =>
            {
                if (finalAssemblies.All(a => a.FullName != assembly.FullName))
                    finalAssemblies.Add(assembly);
            });

            var iPersistenceConfigurer = GetPersistenceConfigurer();
            var config = Fluently.Configure()
                .Database(iPersistenceConfigurer)
                .Mappings(m => m.AutoMappings.Add(AutoMap.Assemblies(new MrCMSMappingConfiguration(), finalAssemblies)
                                                .IgnoreBase<SystemEntity>().IgnoreBase<SiteEntity>()
                                                .IncludeBase<Document>().IncludeBase<Webpage>()
                                                .IncludeBase<Widget>().IncludeBase<FormProperty>()
                                                .IncludeAppBases()
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
            var updateIndexesListener = new UpdateIndexesListener();
            var postCommitEventListener = new PostCommitEventListener();
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
                                          new IPreUpdateEventListener[]
                                              {
                                                  saveOrUpdateListener
                                              });

            configuration.AppendListeners(ListenerType.PostCommitUpdate, new IPostUpdateEventListener[]
                                                                             {
                                                                                 postCommitEventListener
                                                                             });
            if (!InDevelopment && CurrentRequestData.DatabaseIsInstalled)
            {
                configuration.AppendListeners(ListenerType.PostCommitUpdate, new IPostUpdateEventListener[]
                                                                                 {
                                                                                     updateIndexesListener
                                                                                 });
                configuration.AppendListeners(ListenerType.PostCommitInsert, new IPostInsertEventListener[]
                                                                                 {
                                                                                     updateIndexesListener
                                                                                 });
                configuration.AppendListeners(ListenerType.PostCommitDelete, new IPostDeleteEventListener[]
                                                                                 {
                                                                                     updateIndexesListener
                                                                                 });
            }
        }
    }
}