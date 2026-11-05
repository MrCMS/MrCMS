using System;
using Mapster;
using NHibernate;
using NHibernate.Proxy;

namespace MrCMS.Mapping;

/// <summary>
/// Maps using the shared Mapster configuration, exposing the current NHibernate session and
/// request services to mapping rules (see <see cref="MappingContextExtensions"/>).
/// Like the previous AutoMapper implementation, mapping uses the runtime types of the source and
/// destination, so configuration for a subtype (e.g. Article) is picked up when called with a base type.
/// </summary>
public class SessionAwareMapper : ISessionAwareMapper
{
    private readonly TypeAdapterConfig _config;
    private readonly ISession _session;
    private readonly IServiceProvider _serviceProvider;

    public SessionAwareMapper(TypeAdapterConfig config, ISession session, IServiceProvider serviceProvider)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public TDestination Map<TDestination>(object source)
    {
        if (source == null)
            return default;
        source = Unproxy(source);
        return (TDestination)Map(source, source.GetType(), typeof(TDestination));
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
            return destination;
        object sourceObject = Unproxy(source);
        object destinationObject = Unproxy(destination);
        var destinationType = destinationObject?.GetType() ?? typeof(TDestination);

        using var scope = CreateScope();
        return (TDestination)sourceObject.Adapt(destinationObject, sourceObject.GetType(), destinationType, _config);
    }

    public object Map(object source, Type sourceType, Type destinationType)
    {
        if (source == null)
            return null;
        source = Unproxy(source);
        if (sourceType.IsAssignableFrom(source.GetType()))
            sourceType = source.GetType();

        using var scope = CreateScope();
        return source.Adapt(sourceType, destinationType, _config);
    }

    private MapContextScope CreateScope()
    {
        var scope = new MapContextScope();
        scope.Context.Parameters[MappingContextExtensions.SessionKey] = _session;
        scope.Context.Parameters[MappingContextExtensions.ServiceProviderKey] = _serviceProvider;
        return scope;
    }

    private object Unproxy(object entity)
    {
        if (entity is INHibernateProxy)
            return _session.GetSessionImplementation().PersistenceContext.Unproxy(entity);
        return entity;
    }
}
