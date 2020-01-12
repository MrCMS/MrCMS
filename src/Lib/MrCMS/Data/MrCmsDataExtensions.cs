using System;
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
            IConfiguration configSection,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
        {
            serviceCollection.Configure<DatabaseSettings>(configSection.GetSection("Database"));
            serviceCollection.AddSingleton<IDatabaseProvider>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<DatabaseSettings>>();
                if (options.Value != null)
                {
                    var reflectionHelper = provider.GetRequiredService<IReflectionHelper>();
                    var type = reflectionHelper.GetTypeByFullName(options.Value.DatabaseProviderType);
                    return (IDatabaseProvider)provider.GetRequiredService(type);
                }
                return null;
            });

            serviceCollection.AddDbContext<WebsiteContext>((provider, builder) =>
            {
                var databaseProvider = provider.GetRequiredService<IDatabaseProvider>();
                databaseProvider.SetupAction(provider, builder);
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