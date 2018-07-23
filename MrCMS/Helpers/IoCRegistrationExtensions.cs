using System.Linq;
using Microsoft.Extensions.DependencyInjection;

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

        //public static void RegisterSettings(this IServiceCollection container, IReflectionHelper helper)
        //{
        //    foreach (var type in helper.GetAllConcreteImplementationsOf(typeof(ISiteSettings)))
        //        container.AddScoped(type,
        //            provider => provider.GetRequiredService<ISiteConfigurationProvider>().GetSiteSettings(type));
        //}
    }
}