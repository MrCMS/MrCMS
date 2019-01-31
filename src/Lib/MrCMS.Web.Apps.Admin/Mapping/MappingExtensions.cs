using System;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public static class MappingExtensions
    {
        public static IMappingExpression<TSource, TDestination> MapEntityLookup<TSource, TDestination, TEntity>(
            this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TSource, int?>> selector, Expression<Func<TDestination, TEntity>> destinationEntity,
            Expression<Func<TSource, TDestination, bool>> condition = null)
            where TEntity : SystemEntity
        {
            return expression
                .ForMember(destinationEntity,
                    configurationExpression =>
                    {
                        configurationExpression.Ignore();
                    }).AfterMap((source, destination, context) =>
                {
                    if (condition != null && !condition.Compile()(source, destination))
                    {
                        return;
                    }
                    var session = context.Options.CreateInstance<ISession>();
                    var entityPropertyInfo = (destinationEntity.Body as MemberExpression)?.Member as PropertyInfo;
                    var idFunc = selector.Compile();
                    var id = idFunc(source);
                    entityPropertyInfo?.SetValue(destination, id == null ? null : session.Get<TEntity>(id));
                });
        }
    }
}