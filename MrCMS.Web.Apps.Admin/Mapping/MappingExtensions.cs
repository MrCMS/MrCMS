using System;
using System.Linq.Expressions;
using AutoMapper;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public static class MappingExtensions
    {
        public static IMappingExpression<TSource, TDestination> MapEntityLookup<TSource, TDestination, TEntity>(
            this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TSource, int?>> selector, Expression<Func<TDestination, TEntity>> destinationEntity)
            where TEntity : SystemEntity
        {
            return expression.ForMember(destinationEntity,
                configurationExpression =>
                    configurationExpression.ResolveUsing<CustomResolver<TSource, TDestination, TEntity>, int?>(source =>
                        selector.Compile()(source)));
        }
    }
}