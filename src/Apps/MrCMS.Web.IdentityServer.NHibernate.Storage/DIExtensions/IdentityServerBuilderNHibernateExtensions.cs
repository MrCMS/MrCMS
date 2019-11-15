using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.NHibernate.TokenCleanup;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Hosting;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Services;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Stores;
using MrCMS.Web.IdentityServer.NHibernate.Storage.TokenCleanup;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderNHibernateExtensions
    {

        /// <summary>
        /// Configures MrCMS NHibernate-based database support for IdentityServer 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddNHibernateStores(this IIdentityServerBuilder builder)
        {

            // Adds configuration store components
            builder.AddConfigurationStore();

            // Adds operational store components.
            builder.AddOperationalStore();

            return builder;

            
        }


        /// <summary>
        /// Configures MrCMS UserStore for IdentityServer
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddMrCMSUserStore(this IIdentityServerBuilder builder)
        {
            builder.AddUserConfiguration();
            builder.AddProfileService<MrCMSIS4ProfileService>();
            builder.AddResourceOwnerValidator<MrCMSIS4ResourceOwnerPasswordValidator>();

          
            return builder;
        }

        /// <summary>
        /// Adds services for managing IdentityServer user store.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static IIdentityServerBuilder AddUserConfiguration(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IResourceOwnerPasswordValidator, MrCMSIS4ResourceOwnerPasswordValidator>();
            builder.Services.AddTransient<IProfileService, MrCMSIS4ProfileService>();

            return builder;
        }

        /// <summary>
        /// Adds an implementation of the IOperationalStoreNotification to IdentityServer.
        /// </summary>
        /// <typeparam name="T">Concrete implementation of the <see cref="IOperationalStoreNotification"/> interface.</typeparam>
        /// <param name="builder">The builder.</param>
        public static IIdentityServerBuilder AddOperationalStoreNotification<T>(
            this IIdentityServerBuilder builder)
            where T : class, IOperationalStoreNotification
        {
            builder.Services.AddTransient<IOperationalStoreNotification, T>();

            return builder;
        }


       

        /// <summary>
        /// Adds the stores for managing IdentityServer configuration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static IIdentityServerBuilder AddConfigurationStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            return builder;
        }

        /// <summary>
        /// Adds the cache based stores for managing IdentityServer configuration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static IIdentityServerBuilder AddCachedConfigurationStore(this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();
            builder.AddCorsPolicyCache<CorsPolicyService>();

            return builder;
        }

        /// <summary>
        /// Adds stores and services for managing IdentityServer persisted grants.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static IIdentityServerBuilder AddOperationalStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<TokenCleanup>();
            //builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();
            builder.Services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            return builder;
        }

        /// <summary>
        /// Creates database schema from scratch.
        /// </summary>
        /// <param name="databaseConfiguration">The database configuration.</param>
        private static void CreateDatabaseSchema(NHibernate.Cfg.Configuration databaseConfiguration)
        {
            var schemaExport = new SchemaExport(databaseConfiguration);
            schemaExport.Drop(false, true);
            schemaExport.Create(false, true);
        }
    }
}
