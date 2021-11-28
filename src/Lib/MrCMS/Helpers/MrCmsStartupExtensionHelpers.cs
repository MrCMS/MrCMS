using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using MrCMS.Website.Caching;

namespace MrCMS.Helpers
{
    public static class MrCmsStartupExtensionHelpers
    {
        public static IServiceCollection AddRequiredServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterAllSimplePairings();
            serviceCollection.RegisterOpenGenerics();
            serviceCollection.SelfRegisterAllConcreteTypes();
            serviceCollection.AddSession();
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddSingleton<IStandardPriorityMemoryCache>(_ => new StandardPriorityMemoryCache(
                new OptionsWrapper<MemoryCacheOptions>(
                    new MemoryCacheOptions
                        {ExpirationScanFrequency = TimeSpan.FromMinutes(1)})));
            serviceCollection.AddSingleton<IHighPriorityMemoryCache>(_ => new HighPriorityMemoryCache(
                new OptionsWrapper<MemoryCacheOptions>(
                    new MemoryCacheOptions
                        {ExpirationScanFrequency = TimeSpan.FromMinutes(1)})));
            
            serviceCollection.AddHttpClient();
            return serviceCollection;
        }

        public static IServiceCollection AddCultureInfo(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            var settings = new SystemConfig();
            configuration.GetSection("SystemConfigurationSettings").Bind(settings);
            var settingsSupportedCultures = settings.SupportedCultures ?? new List<string>() {"en-GB"};
            var supportedCultures =
                CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Where(x => settingsSupportedCultures.Contains(x.Name)).ToList();

            serviceCollection.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                // options.RequestCultureProviders.Insert(0, new UserProfileRequestCultureProvider());
            });

            return serviceCollection;
        }

        public static void SetDefaultPageSize(this IConfiguration configuration)
        {
            try
            {
                var defaultPageSize = configuration.GetValue<int>("DefaultPageSize");
                SessionHelper.DefaultPageSize = defaultPageSize > 0 ? defaultPageSize : 25;
            }
            catch
            {
                SessionHelper.DefaultPageSize = 25;
            }
        }

        public static IServiceCollection AddSiteProvider(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(provider =>
                provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Items["current-site"] as Site);
            return serviceCollection;
        }
    }
}