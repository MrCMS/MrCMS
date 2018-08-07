using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Settings;

namespace MrCMS.Helpers
{
    public static class IoCRegistrationExtensions
    {
        public static void RegisterAllSimplePairings(this IServiceCollection container)
        {
            var pairings = TypeHelper.GetSimpleInterfaceImplementationPairings();
            foreach (var interfaceType in pairings.Keys)
                container.AddScoped(interfaceType, pairings[interfaceType]);
        }

        public static void RegisterOpenGenerics(this IServiceCollection container)
        {
            var interfaces = TypeHelper.GetAllOpenGenericInterfaces();

            foreach (var interfaceType in interfaces)
                foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom(interfaceType).Where(x => x.IsGenericTypeDefinition))
                    container.AddScoped(interfaceType, type);
        }

        public static void SelfRegisterAllConcreteTypes(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<object>())
                container.AddScoped(type);
        }

        public static void RegisterSettings(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<SystemSettingsBase>())
                container.AddScoped(type,
                    provider =>
                    {
                        var configurationProvider = provider.GetRequiredService<ISystemConfigurationProvider>();
                        var methodInfo = configurationProvider.GetType().GetMethodExt(nameof(ISystemConfigurationProvider.GetSystemSettings));
                        return methodInfo.MakeGenericMethod(type).Invoke(configurationProvider, Array.Empty<object>());
                    });


            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>())
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
}