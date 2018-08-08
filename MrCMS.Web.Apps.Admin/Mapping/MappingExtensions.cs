using System;
using System.Linq.Expressions;
using System.Reflection;
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
            return expression
                .ForMember(destinationEntity,
                    configurationExpression =>
                    {
                        configurationExpression
                            .ResolveUsing<EntityResolver<TSource, TDestination, TEntity>, int?>(
                                source => selector.Compile()(source) 
                            );
                    }).AfterMap((source, destination) =>
                {
                    // TODO: see if we don't need this - basically Automapper appears to be trying to map the Id property on the child object anyway if it exists
                    var entityFunc = destinationEntity.Compile();
                    var entity = entityFunc(destination);
                    if (entity != null && entity.Id <= 0)
                    {
                        var propertyInfo = (destinationEntity.Body as MemberExpression)?.Member as PropertyInfo;

                        propertyInfo?.SetValue(destination, null);
                    }
                });
        }
    }
}