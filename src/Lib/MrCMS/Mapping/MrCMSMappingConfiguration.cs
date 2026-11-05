using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;

namespace MrCMS.Mapping;

public static class MrCMSMappingConfiguration
{
    /// <summary>
    /// Creates the Mapster configuration with defaults that match MrCMS's previous AutoMapper behaviour
    /// and applies each app's mapping registrations.
    /// </summary>
    public static TypeAdapterConfig Create(MrCMSAppContext appContext, params Assembly[] additionalAssemblies)
    {
        var config = new TypeAdapterConfig
        {
            // AutoMapper applied base-type maps to subtypes on both sides (e.g. UpdateWebpageViewModel -> Article)
            AllowImplicitSourceInheritance = true,
            AllowImplicitDestinationInheritance = true,
        };
        config.Default
            // AutoMapper matched member names case-insensitively
            .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)
            // AutoMapper assigned same-typed members by reference; never deep-copy NHibernate entities
            .ShallowCopyForSameType(true);

        appContext?.ConfigureMapping(config);
        foreach (var assembly in additionalAssemblies)
            config.Scan(assembly);

        return config;
    }

    public static IServiceCollection AddMrCMSMapping(this IServiceCollection services, MrCMSAppContext appContext,
        params Assembly[] additionalAssemblies)
    {
        services.AddSingleton(Create(appContext, additionalAssemblies));
        services.AddScoped<ISessionAwareMapper, SessionAwareMapper>();
        return services;
    }
}
