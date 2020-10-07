using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Globalization;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using StackExchange.Profiling.Storage;
using ISession = NHibernate.ISession;

namespace MrCMS.Helpers
{
    public static class MrCmsDataExtensions
    {
        public static IServiceCollection AddMrCMSData(this IServiceCollection services, bool isInstalled, IConfiguration configuration)
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
                services.Configure<DatabaseSettings>(settings =>
                {
                    settings.ConnectionString = configuration.GetConnectionString("mrcms");
                    settings.DatabaseProviderType = configuration.GetSection("Database")
                        .GetValue<string>(nameof(DatabaseSettings.DatabaseProviderType));
                });
                
                services.RegisterDatabaseProviders();

                services.AddSingleton<IDatabaseProviderResolver, DatabaseProviderResolver>();
                services.AddSingleton<ISessionFactory>(provider =>
                    new NHibernateConfigurator(provider.GetRequiredService<IDatabaseProviderResolver>().GetProvider(),
                        provider.GetRequiredService<MrCMSAppContext>()).CreateSessionFactory());
            }

            return services;
        }
    }

    public static class MrCmsStartupExtensionHelpers
    {
        public static IServiceCollection AddRequiredServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterAllSimplePairings();
            serviceCollection.RegisterOpenGenerics();
            serviceCollection.SelfRegisterAllConcreteTypes();
            serviceCollection.AddSession();
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return serviceCollection;
        }
        
        public static IServiceCollection AddCultureInfo(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var settings = new SystemConfig();
            configuration.GetSection("SystemConfigurationSettings").Bind(settings);
            var settingsSupportedCultures = settings.SupportedCultures ?? new List<string>() {"en-GB"};
            var supportedCultures =
                CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Where(x=> settingsSupportedCultures.Contains(x.Name)).ToList();

            serviceCollection.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new UserProfileRequestCultureProvider());
            });

            return serviceCollection;
        }

        public static IServiceCollection AddSiteProvider(this IServiceCollection serviceCollection)
        {
            // TODO: Look to removing Site for constructors and resolving like this
            serviceCollection.AddScoped(provider =>
            {
                var site =
                    provider.GetRequiredService<IHttpContextAccessor>().HttpContext.Items["override-site"] as Site;

                site ??= provider.GetRequiredService<ICurrentSiteLocator>().GetCurrentSite();
                var session = provider.GetRequiredService<ISession>();
                if (site != null)
                {
                    session.EnableFilter("SiteFilter").SetParameter("site", site.Id);
                }
                return site;
            });
            return serviceCollection;
        }
        
        public static IServiceCollection AddMiniProfiler(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMiniProfiler(options =>
            {
                // All of this is optional. You can simply call .AddMiniProfiler() for all defaults

                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) Control storage
                // (default is 30 minutes in MemoryCacheStorage)
                (options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);

                // (Optional) Control which SQL formatter to use, InlineFormatter is the default
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();

                // (Optional) To control authorization, you can use the Func<HttpRequest, bool> options:
                // (default is everyone can access profilers)
                options.ResultsAuthorize = MiniProfilerAuth.IsUserAllowedToSeeMiniProfilerUI;
                options.ResultsListAuthorize = MiniProfilerAuth.IsUserAllowedToSeeMiniProfilerUI;

                // (Optional)  To control which requests are profiled, use the Func<HttpRequest, bool> option:
                // (default is everything should be profiled)
                options.ShouldProfile = MiniProfilerAuth.ShouldStartFor;

                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                options.TrackConnectionOpenClose = true;
            });

            return serviceCollection;
        }

        public static IServiceCollection AddExternalAuthProviders(this IServiceCollection serviceCollection, AuthenticationBuilder builder)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var thirdPartyAuthSettings = serviceProvider.GetRequiredService<ThirdPartyAuthSettings>();

            if (thirdPartyAuthSettings.GoogleEnabled)
            {
                builder.AddGoogle(options =>
                {
                    options.ClientId = thirdPartyAuthSettings.GoogleClientId;
                    options.ClientSecret = thirdPartyAuthSettings.GoogleClientSecret;
                });
            }

            if (thirdPartyAuthSettings.FacebookEnabled)
            {
                builder.AddFacebook(options =>
                {
                    options.AppId = thirdPartyAuthSettings.FacebookAppId;
                    options.AppSecret = thirdPartyAuthSettings.FacebookAppSecret;
                    options.Fields.Add("email");
                    options.Scope.Add("email");
                });
            }

            return serviceCollection;
        }
    }
}