using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MrCMS.Apps;
using MrCMS.Config;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.DbConfiguration.Conventions;
using MrCMS.DbConfiguration.Filters;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Caches.SysCache2;
using NHibernate.Dialect;
using NHibernate.Event;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;
using TypeHelper = MrCMS.Helpers.TypeHelper;

namespace MrCMS.DbConfiguration
{
    public class NHibernateConfigurator
    {
        private List<Assembly> _manuallyAddedAssemblies = new List<Assembly>();

        public DatabaseType DatabaseType { get; set; }
        public bool InDevelopment { get; set; }
        public bool CacheEnabled { get; set; }

        public IPersistenceConfigurer PersistenceOverride { get; set; }

        public List<Assembly> ManuallyAddedAssemblies
        {
            get { return _manuallyAddedAssemblies; }
            set { _manuallyAddedAssemblies = value; }
        }

        public ISessionFactory CreateSessionFactory()
        {
            NHibernate.Cfg.Configuration configuration = GetConfiguration();

            return configuration.BuildSessionFactory();
        }

        private IPersistenceConfigurer GetPersistenceConfigurer()
        {
            if (PersistenceOverride != null)
                return PersistenceOverride;
            switch (DatabaseType)
            {
                case DatabaseType.Auto:
                    ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["mrcms"];
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
            List<Assembly> assemblies = TypeHelper.GetAllMrCMSAssemblies();
            assemblies.AddRange(ManuallyAddedAssemblies);

            var finalAssemblies = new List<Assembly>();

            assemblies.ForEach(assembly =>
                                   {
                                       if (finalAssemblies.All(a => a.FullName != assembly.FullName))
                                           finalAssemblies.Add(assembly);
                                   });

            IPersistenceConfigurer iPersistenceConfigurer = GetPersistenceConfigurer();
            AutoPersistenceModel addFromAssemblyOf =
                AutoMap.Assemblies(new MrCMSMappingConfiguration(), finalAssemblies)
                       .IgnoreBase<SystemEntity>()
                       .IgnoreBase<SiteEntity>()
                       .IncludeBase<Document>()
                       .IncludeBase<Webpage>()
                       .IncludeBase<MessageTemplate>()
                       .IncludeBase<UserProfileData>()
                       .IncludeBase<Widget>()
                       .IncludeBase<FormProperty>()
                       .IncludeBase<FormPropertyWithOptions>()
                       .IncludeAppBases()
                       .UseOverridesFromAssemblies(assemblies.Where(assembly => !assembly.GlobalAssemblyCache).ToArray())
                       .Conventions.AddFromAssemblyOf<CustomForeignKeyConvention>()
                       .IncludeAppConventions();
            addFromAssemblyOf.Add(typeof(NotDeletedFilter));
            addFromAssemblyOf.Add(typeof(SiteFilter));
            NHibernate.Cfg.Configuration config = Fluently.Configure()
                                                          .Database(iPersistenceConfigurer)
                                                          .Mappings(m => m.AutoMappings.Add(addFromAssemblyOf))
                                                          .Cache(builder =>
                                                                     {
                                                                         if (CacheEnabled)
                                                                         {
                                                                             builder.UseSecondLevelCache()
                                                                                    .UseQueryCache()
                                                                                    .QueryCacheFactory
                                                                                 <StandardQueryCacheFactory>();
                                                                             var mrCMSSection =
                                                                                 WebConfigurationManager.GetSection(
                                                                                     "mrcms") as MrCMSConfigSection;
                                                                             if (mrCMSSection != null)
                                                                             {
                                                                                 builder.ProviderClass(
                                                                                     mrCMSSection.CacheProvider
                                                                                                 .AssemblyQualifiedName);
                                                                                 if (mrCMSSection.MinimizePuts)
                                                                                     builder.UseMinimalPuts();
                                                                             }
                                                                             else
                                                                                 builder.ProviderClass<SysCacheProvider>
                                                                                     ();
                                                                         }
                                                                     })
                                                          .ExposeConfiguration(AppendListeners)
                                                          .ExposeConfiguration(AppSpecificConfiguration)
                                                          .ExposeConfiguration(c =>
                                                                                   {
#if DEBUG
                                                                                       c.SetProperty(
                                                                                           Environment
                                                                                               .GenerateStatistics,
                                                                                           "true");
#endif
                                                                                       c.SetProperty(
                                                                                           Environment.Hbm2ddlKeyWords,
                                                                                           "auto-quote");
                                                                                       //c.SetProperty(
                                                                                       //    Environment
                                                                                       //        .DefaultBatchFetchSize,
                                                                                       //    "25");
                                                                                       c.SetProperty(
                                                                                           Environment.BatchSize, "25");
                                                                                   })
                                                          .BuildConfiguration();


            ValidateSchema(config);

            config.BuildMappings();

            return config;
        }

        private void AppSpecificConfiguration(NHibernate.Cfg.Configuration configuration)
        {
            MrCMSApp.AppendAllAppConfiguration(configuration);
        }

        private void AppendListeners(NHibernate.Cfg.Configuration configuration)
        {
            configuration.EventListeners.SaveOrUpdateEventListeners =
                new ISaveOrUpdateEventListener[]
                    {
                        MrCMSListeners.SaveOrUpdateListener
                    };

            configuration.AppendListeners(ListenerType.PreInsert,
                                          new[]
                                              {
                                                  MrCMSListeners.SaveOrUpdateListener
                                              });
            configuration.AppendListeners(ListenerType.PreUpdate,
                                          new IPreUpdateEventListener[]
                                              {
                                                  MrCMSListeners.SaveOrUpdateListener
                                              });

            configuration.AppendListeners(ListenerType.PostCommitUpdate, new IPostUpdateEventListener[]
                                                                             {
                                                                                 MrCMSListeners.PostCommitEventListener, MrCMSListeners.UrlHistoryListener
                                                                             });

            var softDeleteListener = new SoftDeleteListener(InDevelopment);
            configuration.SetListener(ListenerType.Delete, softDeleteListener);

            if (!InDevelopment && CurrentRequestData.DatabaseIsInstalled)
            {
                configuration.AppendListeners(ListenerType.PostCommitUpdate, new IPostUpdateEventListener[]
                                                                                 {
                                                                                     MrCMSListeners.UpdateIndexesListener
                                                                                 });
                configuration.AppendListeners(ListenerType.PostCommitInsert, new IPostInsertEventListener[]
                                                                                 {
                                                                                     MrCMSListeners.UpdateIndexesListener
                                                                                 });
                configuration.AppendListeners(ListenerType.PostCommitDelete, new IPostDeleteEventListener[]
                                                                                 {
                                                                                     MrCMSListeners.UpdateIndexesListener
                                                                                 });
                configuration.AppendListeners(ListenerType.PostCollectionRecreate,
                                              new IPostCollectionRecreateEventListener[]
                                                  {
                                                      MrCMSListeners.UpdateIndexesListener
                                                  });
                configuration.AppendListeners(ListenerType.PostCollectionRemove,
                                              new IPostCollectionRemoveEventListener[]
                                                  {
                                                      MrCMSListeners.UpdateIndexesListener
                                                  });
                configuration.AppendListeners(ListenerType.PostCollectionUpdate,
                                              new IPostCollectionUpdateEventListener[]
                                                  {
                                                      MrCMSListeners.UpdateIndexesListener
                                                  });
            }
        }
    }
}