using System;

namespace MrCMS.Mapping;

public interface ISessionAwareMapper
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    object Map(object source, Type sourceType, Type destinationType);
}