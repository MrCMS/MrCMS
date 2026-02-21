using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Shortcodes.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using MrCMS.ContentTemplates.ContentTemplateTokenProviders.Base;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Entities.Documents.Web;
using MrCMS.Shortcodes;

namespace MrCMS.Helpers
{
    public static class IoCRegistrationExtensions
    {
        /// <summary>
        /// Single-pass type classification and registration for all service-specific
        /// registrations (settings, renderers, token providers, metadata, etc.).
        /// Replaces 8+ separate type scans with one iteration over all types.
        /// Core registrations (simple pairings, open generics, self-register) are
        /// handled separately via AddRequiredServices since they run unconditionally.
        /// </summary>
        public static void RegisterAllDiscoveredServices(this IServiceCollection container)
        {
            var allTypes = TypeHelper.GetAllTypes();

            var systemSettingsBaseType = typeof(SystemSettingsBase);
            var siteSettingsBaseType = typeof(SiteSettingsBase);
            var contentTemplateTokenProviderType = typeof(ContentTemplateTokenProvider);
            var shortcodeRendererType = typeof(IShortcodeRenderer);
            var routeTransformerType = typeof(DynamicRouteValueTransformer);
            var tokenProviderType = typeof(ITokenProvider);
            var tokenProviderGenericDef = typeof(ITokenProvider<>);
            var webpageMetadataInfoType = typeof(IGetWebpageMetadataInfo);
            var databaseProviderType = typeof(IDatabaseProvider);
            var formPropertyType = typeof(FormProperty);
            var formElementRendererGenericDef = typeof(IFormElementRenderer<>);

            var systemSettings = new List<Type>();
            var siteSettings = new List<Type>();
            var contentTemplateTokenProviders = new List<Type>();
            var formProperties = new List<Type>();
            var formRenderers = new List<Type>();
            var shortcodeRenderers = new List<Type>();
            var routeTransformers = new List<Type>();
            var tokenProviders = new List<Type>();
            var genericTokenProviders = new List<Type>();
            var webpageMetadataInfos = new List<Type>();
            var databaseProviders = new List<Type>();

            foreach (var type in allTypes)
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                if (!type.IsGenericTypeDefinition)
                {
                    if (systemSettingsBaseType.IsAssignableFrom(type))
                        systemSettings.Add(type);
                    if (siteSettingsBaseType.IsAssignableFrom(type))
                        siteSettings.Add(type);
                    if (contentTemplateTokenProviderType.IsAssignableFrom(type))
                        contentTemplateTokenProviders.Add(type);
                    if (shortcodeRendererType.IsAssignableFrom(type))
                        shortcodeRenderers.Add(type);
                    if (routeTransformerType.IsAssignableFrom(type))
                        routeTransformers.Add(type);
                    if (tokenProviderType.IsAssignableFrom(type))
                        tokenProviders.Add(type);
                    if (webpageMetadataInfoType.IsAssignableFrom(type))
                        webpageMetadataInfos.Add(type);
                    if (databaseProviderType.IsAssignableFrom(type))
                        databaseProviders.Add(type);
                    if (formPropertyType.IsAssignableFrom(type))
                        formProperties.Add(type);

                    foreach (var iface in type.GetInterfaces())
                    {
                        if (!iface.IsGenericType) continue;
                        var genericDef = iface.GetGenericTypeDefinition();
                        if (genericDef == formElementRendererGenericDef)
                            formRenderers.Add(type);
                        if (genericDef == tokenProviderGenericDef)
                            genericTokenProviders.Add(type);
                    }
                }
                else
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                        if (!iface.IsGenericType) continue;
                        if (iface.GetGenericTypeDefinition() == tokenProviderGenericDef)
                            genericTokenProviders.Add(type);
                    }
                }
            }

            RegisterSettingsTypes(container, systemSettings, siteSettings);
            RegisterContentTemplateTokenProviderTypes(container, contentTemplateTokenProviders);
            RegisterFormRendererTypes(container, formProperties, formRenderers, formElementRendererGenericDef);
            RegisterShortcodeRendererTypes(container, shortcodeRenderers);
            RegisterRouteTransformerTypes(container, routeTransformers);
            RegisterTokenProviderTypes(container, tokenProviders, genericTokenProviders, tokenProviderGenericDef);
            RegisterWebpageMetadataTypes(container, webpageMetadataInfos);
            RegisterDatabaseProviderTypes(container, databaseProviders);
        }

        private static void RegisterSettingsTypes(
            IServiceCollection container,
            List<Type> systemSettings,
            List<Type> siteSettings)
        {
            foreach (var type in systemSettings)
            {
                container.AddScoped(type, provider =>
                {
                    var configurationProvider = provider.GetRequiredService<ISystemConfigurationProvider>();
                    var methodInfo = configurationProvider.GetType()
                        .GetMethodExt(nameof(ISystemConfigurationProvider.GetSystemSettings));
                    return methodInfo.MakeGenericMethod(type).Invoke(configurationProvider, Array.Empty<object>());
                });
            }

            foreach (var type in siteSettings)
            {
                container.AddScoped(type, provider =>
                {
                    var configurationProvider = provider.GetRequiredService<IConfigurationProvider>();
                    var methodInfo = configurationProvider.GetType()
                        .GetMethodExt(nameof(IConfigurationProvider.GetSiteSettings));
                    return methodInfo.MakeGenericMethod(type).Invoke(configurationProvider, Array.Empty<object>());
                });
            }
        }

        private static void RegisterContentTemplateTokenProviderTypes(
            IServiceCollection container,
            List<Type> types)
        {
            foreach (var type in types)
                container.AddScoped(typeof(ContentTemplateTokenProvider), type);
        }

        private static void RegisterFormRendererTypes(
            IServiceCollection container,
            List<Type> formProperties,
            List<Type> formRenderers,
            Type formElementRendererGenericDef)
        {
            foreach (var formPropType in formProperties)
            {
                var rendererInterfaceType = formElementRendererGenericDef.MakeGenericType(formPropType);
                var concreteType = formRenderers.FirstOrDefault(x => rendererInterfaceType.IsAssignableFrom(x));
                if (concreteType != null)
                    container.AddScoped(rendererInterfaceType, concreteType);
            }
        }

        private static void RegisterShortcodeRendererTypes(
            IServiceCollection container,
            List<Type> types)
        {
            foreach (var type in types)
            {
                if (!container.Any(x => x.ServiceType == typeof(IShortcodeRenderer) && x.ImplementationType == type))
                    container.AddScoped(typeof(IShortcodeRenderer), type);
            }
        }

        private static void RegisterRouteTransformerTypes(
            IServiceCollection container,
            List<Type> types)
        {
            foreach (var type in types)
                container.AddTransient(type);
        }

        private static void RegisterTokenProviderTypes(
            IServiceCollection container,
            List<Type> tokenProviders,
            List<Type> genericTokenProviders,
            Type tokenProviderGenericDef)
        {
            foreach (var type in tokenProviders)
            {
                if (!container.Any(x => x.ServiceType == typeof(ITokenProvider) && x.ImplementationType == type))
                    container.AddScoped(typeof(ITokenProvider), type);
            }

            foreach (var type in genericTokenProviders)
            {
                if (type.IsGenericType)
                {
                    if (!container.Any(x =>
                            x.ServiceType == tokenProviderGenericDef && x.ImplementationType == type))
                        container.AddScoped(tokenProviderGenericDef, type);
                }
                else
                {
                    var typed = type.GetBaseTypes(true).SelectMany(x => x.GetInterfaces())
                        .FirstOrDefault(x =>
                            x.IsGenericType && x.GetGenericTypeDefinition() == tokenProviderGenericDef);
                    if (typed != null &&
                        !container.Any(x => x.ServiceType == typed && x.ImplementationType == type))
                        container.AddScoped(typed, type);
                }
            }
        }

        private static void RegisterWebpageMetadataTypes(
            IServiceCollection container,
            List<Type> types)
        {
            foreach (var type in types)
            {
                if (!type.IsGenericType)
                {
                    container.AddSingleton(typeof(IGetWebpageMetadataInfo), type);
                    container.AddSingleton(type, type);
                }
                else
                {
                    foreach (var webpageType in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                                 .Where(t => !t.ContainsGenericParameters))
                    {
                        var genericType = type.MakeGenericType(webpageType);
                        container.AddSingleton(genericType, genericType);
                    }
                }
            }
        }

        private static void RegisterDatabaseProviderTypes(
            IServiceCollection container,
            List<Type> types)
        {
            foreach (var type in types)
            {
                container.AddTransient(typeof(IDatabaseProvider), type);
                container.AddTransient(type);
            }
        }

        public static void RegisterSiteLocator(this IServiceCollection container)
        {
            container.AddScoped<ICurrentSiteLocator, ContextCurrentSiteLocator>();
        }

        #region Legacy individual registration methods (kept for backward compatibility)

        public static void RegisterAllSimplePairings(this IServiceCollection container)
        {
            var pairings = TypeHelper.GetSimpleInterfaceImplementationPairings();
            foreach (var interfaceType in pairings.Keys)
            {
                if (!container.Any(x =>
                        x.ServiceType == interfaceType && x.ImplementationType == pairings[interfaceType]))
                    container.AddScoped(interfaceType, pairings[interfaceType]);
            }
        }

        public static void RegisterOpenGenerics(this IServiceCollection container)
        {
            var interfaces = TypeHelper.GetAllOpenGenericInterfaces();

            foreach (var interfaceType in interfaces)
            {
                foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFromGeneric(interfaceType)
                             .Where(x => x.IsGenericTypeDefinition))
                {
                    container.AddScoped(interfaceType, type);
                }
            }
        }

        public static void SelfRegisterAllConcreteTypes(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypes())
            {
                if (container.All(x => x.ServiceType != type))
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
                        var methodInfo = configurationProvider.GetType()
                            .GetMethodExt(nameof(ISystemConfigurationProvider.GetSystemSettings));
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

        public static void RegisterContentTemplateTokenProvider(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<ContentTemplateTokenProvider>())
                container.AddScoped(typeof(ContentTemplateTokenProvider), type);
        }

        public static void RegisterFormRenderers(this IServiceCollection container)
        {
            var allRenderers = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(IFormElementRenderer<>));
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<FormProperty>())
            {
                var rendererInterfaceType = typeof(IFormElementRenderer<>).MakeGenericType(type);
                var concreteType = allRenderers.FirstOrDefault(x => rendererInterfaceType.IsAssignableFrom(x));
                if (concreteType != null)
                {
                    container.AddScoped(rendererInterfaceType, concreteType);
                }
            }
        }

        public static void RegisterShortCodeRenderers(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<IShortcodeRenderer>())
            {
                if (!container.Any(x => x.ServiceType == typeof(IShortcodeRenderer) && x.ImplementationType == type))
                    container.AddScoped(typeof(IShortcodeRenderer), type);
            }
        }

        public static void RegisterRouteTransformers(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<DynamicRouteValueTransformer>())
            {
                container.AddTransient(type);
            }
        }

        public static void RegisterTokenProviders(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<ITokenProvider>())
            {
                if (!container.Any(x => x.ServiceType == typeof(ITokenProvider) && x.ImplementationType == type))
                    container.AddScoped(typeof(ITokenProvider), type);
            }

            var tokenProviderGenericType = typeof(ITokenProvider<>);
            var tokenProviderTypes = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(tokenProviderGenericType);
            foreach (var type in tokenProviderTypes)
            {
                if (type.IsGenericType)
                {
                    if (!container.Any(x =>
                            x.ServiceType == tokenProviderGenericType && x.ImplementationType == type))
                        container.AddScoped(tokenProviderGenericType, type);
                }
                else
                {
                    var typed = type.GetBaseTypes(true).SelectMany(x => x.GetInterfaces())
                        .FirstOrDefault(x => x.GetGenericTypeDefinition() == tokenProviderGenericType);
                    if (typed != null &&
                        !container.Any(x => x.ServiceType == typed && x.ImplementationType == type))
                    {
                        container.AddScoped(typed, type);
                    }
                }
            }
        }

        public static void RegisterWebpageMetadata(this IServiceCollection container)
        {
            foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<IGetWebpageMetadataInfo>())
            {
                if (!type.IsGenericType)
                {
                    container.AddSingleton(typeof(IGetWebpageMetadataInfo), type);
                    container.AddSingleton(type, type);
                }
                else
                {
                    foreach (
                        var webpageType in
                        TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                            .Where(type => !type.ContainsGenericParameters))
                    {
                        var genericType = type.MakeGenericType(webpageType);
                        container.AddSingleton(genericType, genericType);
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

        #endregion
    }
}