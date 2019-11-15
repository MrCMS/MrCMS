using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Shortcodes.Forms;
using MrCMS.Tasks;
using System;
using System.Linq;

namespace MrCMS.Helpers
{
    public static class IoCRegistrationExtensions
    {
        public static void RegisterAllSimplePairings(this IServiceCollection container)
        {
            var pairings = TypeHelper.GetSimpleInterfaceImplementationPairings();
            foreach (var interfaceType in pairings.Keys)
            {
                container.AddScoped(interfaceType, pairings[interfaceType]);
            }
        }

        public static void RegisterOpenGenerics(this IServiceCollection container)
        {
            var interfaces = TypeHelper.GetAllOpenGenericInterfaces();

            foreach (var interfaceType in interfaces)
            {
                foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom(interfaceType).Where(x => x.IsGenericTypeDefinition))
                {
                    container.AddScoped(interfaceType, type);
                }
            }
        }

        public static void SelfRegisterAllConcreteTypes(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<object>())
            {
                container.AddScoped(type);
            }
        }

        public static void RegisterSettings(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<SystemSettingsBase>())
            {
                container.AddScoped(type,
                    provider =>
                    {
                        var configurationProvider = provider.GetRequiredService<ISystemConfigurationProvider>();
                        var methodInfo = configurationProvider.GetType().GetMethodExt(nameof(ISystemConfigurationProvider.GetSystemSettings));
                        return methodInfo.MakeGenericMethod(type).Invoke(configurationProvider, Array.Empty<object>());
                    });
            }

            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>())
            {
                container.AddScoped(type,
                    provider =>
                    {
                        var configurationProvider = provider.GetRequiredService<IConfigurationProvider>();
                        var methodInfo = configurationProvider.GetType()
                            .GetMethodExt(nameof(IConfigurationProvider.GetSiteSettings));
                        return methodInfo.MakeGenericMethod(type).Invoke(configurationProvider, Array.Empty<object>());
                    });
            }
        }
        public static void RegisterFormRenderers(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<FormProperty>())
            {
                var rendererInterfaceType = typeof(IFormElementRenderer<>).MakeGenericType(type);
                var concreteType = TypeHelper.GetAllConcreteTypesAssignableFrom(rendererInterfaceType).FirstOrDefault();
                if (concreteType != null)
                {
                    container.AddScoped(rendererInterfaceType, concreteType);
                }
            }
        }

        public static void RegisterTokenProviders(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<ITokenProvider>())
            {
                container.AddScoped(typeof(ITokenProvider), type);
            }

            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(ITokenProvider<>)))
            {
                if (type.IsGenericType)
                {
                    container.AddScoped(typeof(ITokenProvider<>), type);
                }
                else
                {
                    var typed = type.GetBaseTypes(true).SelectMany(x => x.GetInterfaces())
                        .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ITokenProvider<>));
                    if (typed != null)
                    {
                        container.AddScoped(typed, type);
                    }
                }
            }
        }
        public static void RegisterDatabaseProviders(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<IDatabaseProvider>())
            {
                container.AddTransient(typeof(IDatabaseProvider), type);
                container.AddTransient(type);
            }

        }

        public static void RegisterTasks(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<AdHocTask>())
            {
                container.AddTransient(type);
            }
        }


       

       
    }
}