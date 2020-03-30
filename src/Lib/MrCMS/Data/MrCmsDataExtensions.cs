using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MrCMS.Common;
using MrCMS.DbConfiguration;
using MrCMS.Settings;

namespace MrCMS.Data
{
    public static class MrCmsDataExtensions
    {
        public static IServiceCollection AddMrCMSData(this IServiceCollection serviceCollection,
            IReflectionHelper reflectionHelper,
            IConfiguration configSection,
            Assembly migrationsAssembly,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped) //todo: confirm service lifetime
        {
            serviceCollection.Configure<DatabaseSettings>(settings =>
            {
                settings.ConnectionString = configSection.GetConnectionString("mrcms");
                settings.DatabaseProviderType = configSection.GetSection("Database")
                    .GetValue<string>(nameof(DatabaseSettings.DatabaseProviderType));
            });
            serviceCollection.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<DatabaseSettings>>();
                if (options.Value == null) return null;
                var reflectionHelper = provider.GetRequiredService<IReflectionHelper>();
                var type = reflectionHelper.GetTypeByFullName(options.Value.DatabaseProviderType);
                return (IDatabaseProvider)provider.GetRequiredService(type);
            });

            foreach (var type in reflectionHelper.GetAllConcreteImplementationsOf<IDatabaseProvider>())
            {
                serviceCollection.AddSingleton(type);
            }


            serviceCollection.AddDbContext<WebsiteContext>((provider, builder) =>
            {
                var databaseProvider = provider.GetRequiredService<IDatabaseProvider>();
                databaseProvider.SetupAction(provider, builder, migrationsAssembly);
                if (optionsAction == null)
                    return;
                optionsAction(provider, builder);
            }, contextLifetime);

            serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            serviceCollection.AddScoped(typeof(IGlobalRepository<>), typeof(GlobalRepository<>));

            serviceCollection.AddScoped(typeof(IQueryableRepository<>), typeof(QueryableRepository<>));

            serviceCollection.AddScoped(typeof(IJoinTableRepository<>), typeof(JoinTableRepository<>));

            return serviceCollection;
        }
    }
}