using System;
using AutoMapper;
using NHibernate;

namespace MrCMS.Mapping;

public class SessionAwareMapper : ISessionAwareMapper
{
    private readonly IMapper _mapper;
    private readonly ISession _session;

    public SessionAwareMapper(IMapper mapper, ISession session)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source, options => { options.Items["Session"] = _session; });
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return _mapper.Map<TSource, TDestination>(source, options => { options.Items["Session"] = _session; });
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return _mapper.Map(source, destination, options => { options.Items["Session"] = _session; });
    }

    public object Map(object source, Type sourceType, Type destinationType)
    {
        return _mapper.Map(source, sourceType, destinationType, options => { options.Items["Session"] = _session; });
    }
}