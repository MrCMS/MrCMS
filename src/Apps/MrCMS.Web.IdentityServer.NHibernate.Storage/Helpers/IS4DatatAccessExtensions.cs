using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Helpers
{
    public static class IS4DatatAccessExtensions
    {
        public static IServiceCollection AddMrCMSDataAccessToIS4(this IServiceCollection services, bool isInstalled,
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
                        new MrCMSAppContext()).CreateSessionFactory());

                services.AddSingleton<ISessionFactory>(provider =>
                    new NHibernateConfigurator(provider.GetRequiredService<IDatabaseProviderResolver>().GetProvider(),
                        new MrCMSAppContext()).CreateSessionFactory());
            }

            return services;
        }
    }
}
