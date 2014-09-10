using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Caches.SysCache2;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Tool.hbm2ddl;

namespace MrCMS.DbConfiguration
{
    public class NHibernateConfigurator
    {
        private readonly IDatabaseProvider _databaseProvider;
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;
        private List<Assembly> _manuallyAddedAssemblies = new List<Assembly>();

        public NHibernateConfigurator(IDatabaseProvider databaseProvider, ISystemConfigurationProvider systemConfigurationProvider = null)
        {
            _databaseProvider = databaseProvider;
            _systemConfigurationProvider = systemConfigurationProvider;
            CacheEnabled = true;
        }

        public bool CacheEnabled { get; set; }

        public List<Assembly> ManuallyAddedAssemblies
        {
            get { return _manuallyAddedAssemblies; }
            set { _manuallyAddedAssemblies = value; }
        }

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
            HashSet<Assembly> assemblies = TypeHelper.GetAllMrCMSAssemblies();
            assemblies.AddRange(ManuallyAddedAssemblies);

            var finalAssemblies = new List<Assembly>();

            assemblies.ForEach(assembly =>
            {
                if (finalAssemblies.All(a => a.FullName != assembly.FullName))
                    finalAssemblies.Add(assembly);
            });

            IPersistenceConfigurer iPersistenceConfigurer = _databaseProvider.GetPersistenceConfigurer();
            AutoPersistenceModel autoPersistenceModel =
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

            autoPersistenceModel.Add(typeof(NotDeletedFilter));
            autoPersistenceModel.Add(typeof(SiteFilter));
            NHibernate.Cfg.Configuration config = Fluently.Configure()
                .Database(iPersistenceConfigurer)
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel))
                .Cache(builder =>
                {
                    if (CacheEnabled)
                    {
                        builder.UseSecondLevelCache()
                            .UseQueryCache()
                            .QueryCacheFactory<StandardQueryCacheFactory>();
                        var mrCMSSection = WebConfigurationManager.GetSection("mrcms") as MrCMSConfigSection;
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
            configuration.AppendListeners(ListenerType.PreInsert,
                new[]
                {
                    new SetCoreProperties()
                });
            var softDeleteListener = new SoftDeleteListener();
            configuration.SetListener(ListenerType.Delete, softDeleteListener);
        }
    }
}