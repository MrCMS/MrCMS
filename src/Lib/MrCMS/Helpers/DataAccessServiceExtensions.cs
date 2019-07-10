using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;
using ISession = NHibernate.ISession;

namespace MrCMS.Helpers
{
    public static class FileSystemServiceExtensions
    {
        public static IServiceCollection AddMrCMSFileSystem(this IServiceCollection services)
        {
            services.AddScoped<IFileSystem>(provider =>
            {
                var settings = provider.GetService<FileSystemSettings>();

                var storageType = settings.StorageType;
                if (string.IsNullOrWhiteSpace(storageType))
                {
                    return provider.GetService<FileSystem>();
                }

                var type = TypeHelper.GetTypeByName(storageType);
                if (type?.IsAssignableFrom(typeof(IFileSystem)) != true)
                {
                    return provider.GetService(type) as IFileSystem;
                }

                return provider.GetService(type) as IFileSystem;
            });

            return services;
        }

    }
    public static class DataAccessServiceExtensions
    {
        public static IServiceCollection AddMrCMSDataAccess(this IServiceCollection services, bool isInstalled,
            IConfigurationSection settings)
        {
            services.AddScoped<ISession>(provider =>
            {
                var session = provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Items?["override-nh-session"] as ISession;
                return session ?? provider.GetRequiredService<ISessionFactory>().OpenFilteredSession(provider);
            });
            services.AddTransient<IStatelessSession>(provider =>
            {
                var session = provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Items?["override-nh-stateless-session"] as IStatelessSession;
                return session ?? provider.GetRequiredService<ISessionFactory>().OpenStatelessSession();
            });

            if (isInstalled)
            {
                services.Configure<DatabaseSettings>(settings);
                services.RegisterDatabaseProviders();

                services.AddSingleton<IDatabaseProviderResolver, DatabaseProviderResolver>();
                services.AddSingleton<ISessionFactory>(provider =>
                    new NHibernateConfigurator(provider.GetRequiredService<IDatabaseProviderResolver>().GetProvider(),
                        provider.GetRequiredService<MrCMSAppContext>()).CreateSessionFactory());
            }

            return services;
        }
    }
}