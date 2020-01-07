using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Common;
using MrCMS.DbConfiguration;

namespace MrCMS.Data
{
    public static class MrCmsDataExtensions
    {
        public static IServiceCollection AddMrCMSData(this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
        {
            serviceCollection.AddDbContext<WebsiteContext>((provider, builder) =>
            {
                if (optionsAction == null)
                    return;
                optionsAction(builder);
            }, contextLifetime);

            serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            serviceCollection.AddScoped(typeof(IGlobalRepository<>), typeof(GlobalRepository<>));
            serviceCollection.AddScoped(typeof(IQueryableRepository<>), typeof(QueryableRepository<>));
            serviceCollection.AddScoped(typeof(IJoinTableRepository<>), typeof(JoinTableRepository<>));

            return serviceCollection;
        }
    }
}