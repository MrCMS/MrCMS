using System;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;

namespace MrCMS.Mapping;

public static class MappingContextExtensions
{
    public const string SessionKey = "MrCMS.Session";
    public const string ServiceProviderKey = "MrCMS.ServiceProvider";

    /// <summary>
    /// The NHibernate session for the current mapping, or null when mapping outside an <see cref="ISessionAwareMapper"/>.
    /// </summary>
    public static ISession GetSession(this MapContext context)
    {
        return context?.Parameters.TryGetValue(SessionKey, out var session) == true ? session as ISession : null;
    }

    /// <summary>
    /// Creates <typeparamref name="T"/> using the request's services (replaces AutoMapper's DI-constructed value resolvers).
    /// </summary>
    public static T CreateWithServices<T>(this MapContext context)
    {
        if (context?.Parameters.TryGetValue(ServiceProviderKey, out var value) != true ||
            value is not IServiceProvider serviceProvider)
            throw new InvalidOperationException(
                $"Cannot create {typeof(T).Name}: mapping must be performed through ISessionAwareMapper.");
        return ActivatorUtilities.CreateInstance<T>(serviceProvider);
    }
}
