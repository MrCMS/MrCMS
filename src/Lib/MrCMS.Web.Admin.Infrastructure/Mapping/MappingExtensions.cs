using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Web.Admin.Infrastructure.Mapping
{
    public static class MappingExtensions
    {
        public static IMappingExpression<TSource, TDestination> MapEntityLookup<TSource, TDestination, TEntity>(
            this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TSource, int?>> selector,
            Expression<Func<TDestination, TEntity>> destinationEntity,
            Expression<Func<TSource, TDestination, bool>> condition = null)
            where TEntity : SystemEntity
        {
            return expression
                .ForMember(destinationEntity,
                    configurationExpression => configurationExpression.Ignore()).AfterMap(
                    (source, destination, context) =>
                    {
                        if (condition != null && !condition.Compile()(source, destination))
                        {
                            return;
                        }

                        var session = context.Items["Session"] as ISession;
                        if (session == null)
                            return;
                        var entityPropertyInfo = (destinationEntity.Body as MemberExpression)?.Member as PropertyInfo;
                        var idFunc = selector.Compile();
                        var id = idFunc(source);
                        entityPropertyInfo?.SetValue(destination, id == null ? null : session.Get<TEntity>(id));
                    });
        }

        public static IMappingExpression MapEntityLookup(
            this IMappingExpression expression,
            string sourcePropertyName,
            string destinationPropertyName)
        {
            return expression
                .ForMember(destinationPropertyName,
                    configurationExpression => configurationExpression.Ignore()).AfterMap(
                    (source, destination, context) =>
                    {
                        var session = context.Items["Session"] as ISession;
                        if (session == null)
                            return;

                        var sourceProperty = source?.GetType().GetProperties()
                            .FirstOrDefault(x => x.Name == sourcePropertyName);
                        var destinationProperty = destination?.GetType().GetProperties()
                            .FirstOrDefault(x => x.Name == destinationPropertyName);

                        if (sourceProperty == null || destinationProperty == null ||
                            !typeof(SystemEntity).IsAssignableFrom(destinationProperty.PropertyType))
                            return;

                        var id = sourceProperty.GetValue(source);
                        destinationProperty.SetValue(destination,
                            id == null ? null : session.Get(destinationProperty.PropertyType, id));
                    });
        }
    }
}