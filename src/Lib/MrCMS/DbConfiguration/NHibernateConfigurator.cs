using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MrCMS.Apps;
using MrCMS.Batching.Entities;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.DbConfiguration.Conventions;
using MrCMS.DbConfiguration.Filters;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Caches.CoreMemoryCache;
using NHibernate.Event;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

namespace MrCMS.DbConfiguration
{
    public class NHibernateConfigurator
    {
        private readonly IDatabaseProvider _databaseProvider;
        private readonly MrCMSAppContext _appContext;
        private readonly Action<CacheSettingsBuilder> _configureCache;

        public NHibernateConfigurator(IDatabaseProvider databaseProvider, MrCMSAppContext appContext, Action<CacheSettingsBuilder> configureCache = null)
        {
            _databaseProvider = databaseProvider;
            _appContext = appContext;
            _configureCache = configureCache;
        }

        public List<Assembly> ManuallyAddedAssemblies { get; set; }

        public ISessionFactory CreateSessionFactory()
        {
            var configuration = GetConfiguration();

            var sessionFactory = configuration.BuildSessionFactory();

            return sessionFactory;
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
            var assemblies = GetAssemblies();

            if (_databaseProvider == null)
                throw new Exception("Please set the database provider in mrcms.config");

            var iPersistenceConfigurer = _databaseProvider.GetPersistenceConfigurer();
            var autoPersistenceModel = GetAutoPersistenceModel(assemblies);

            var config = Fluently.Configure()
                .Database(iPersistenceConfigurer)
                .Mappings(m =>
                {
                    m.AutoMappings.Add(autoPersistenceModel);
                    foreach (var assembly in assemblies)
                        m.FluentMappings.AddFromAssembly(assembly);
                })

                .Cache(SetupCache)
                .ExposeConfiguration(AppendListeners)
                .ExposeConfiguration(AppSpecificConfiguration)
                .ExposeConfiguration(c =>
                {
#if DEBUG
                    c.SetProperty(Environment.GenerateStatistics, "true");
#else
                    c.SetProperty(Environment.GenerateStatistics, "false");
#endif
                    c.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
                    c.SetProperty(Environment.BatchSize, "25");
                })
                .BuildConfiguration();


            _databaseProvider.AddProviderSpecificConfiguration(config);

            ValidateSchema(config);

            config.BuildMappings();

            return config;
        }

        private List<Assembly> GetAssemblies()
        {
            var assemblies = TypeHelper.GetAllMrCMSAssemblies();
            if (ManuallyAddedAssemblies != null)
                assemblies.AddRange(ManuallyAddedAssemblies);

            var finalAssemblies = new List<Assembly>();

            assemblies.ForEach(assembly =>
            {
                if (finalAssemblies.All(a => a.FullName != assembly.FullName))
                    finalAssemblies.Add(assembly);
            });
            return finalAssemblies;
        }

        private void SetupCache(CacheSettingsBuilder builder)
        {
            //if (!CacheEnabled)
            //    return;
            if (_configureCache != null)
            {
                _configureCache(builder);
            }
            else
            {
                builder.UseSecondLevelCache()
                    .UseQueryCache()
                    .QueryCacheFactory<StandardQueryCacheFactory>();
                builder.ProviderClass<CoreMemoryCacheProvider>();
            }
        }

        private AutoPersistenceModel GetAutoPersistenceModel(List<Assembly> finalAssemblies)
        {
            return AutoMap.Assemblies(new MrCMSMappingConfiguration(), finalAssemblies)
                .IgnoreBase<SystemEntity>()
                .IgnoreBase<SiteEntity>()
                .IncludeBase<Document>()
                .IncludeBase<Webpage>()
                .IncludeBase<UserProfileData>()
                .IncludeBase<Widget>()
                .IncludeBase<FormProperty>()
                .IncludeBase<FormPropertyWithOptions>()
                .IncludeBase<BatchJob>()
                .IncludeBase<ContentBlock>()
                .UseOverridesFromAssemblies(finalAssemblies)
                .Conventions.AddFromAssemblyOf<CustomForeignKeyConvention>()
                .IncludeAppBases(_appContext)
                .IncludeAppConventions(_appContext);
        }

        private void AppSpecificConfiguration(NHibernate.Cfg.Configuration configuration)
        {
            _appContext.AppendConfiguration(configuration);
        }

        private void AppendListeners(NHibernate.Cfg.Configuration configuration)
        {
            configuration.AppendListeners(ListenerType.PreInsert, new[]
            {
                new SetCoreProperties()
            });
            var softDeleteListener = new SoftDeleteListener();
            configuration.SetListener(ListenerType.Delete, softDeleteListener);
        }
    }
}