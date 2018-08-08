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
            Expression<Func<TSource, int?>> selector, Expression<Func<TDestination, TEntity>> destinationEntity)
            where TEntity : SystemEntity
        {
            return expression
                .ForMember(destinationEntity,
                    configurationExpression =>
                    {
                        configurationExpression.Ignore();
                        //configurationExpression.Condition(source => selector.Compile()(source).HasValue);
                        //configurationExpression
                        //    .ResolveUsing<EntityResolver<TSource, TDestination, TEntity>, int?>(
                        //        source => selector.Compile()(source) 
                        //    );
                    }).AfterMap((source, destination, context) =>
                {
                    // TODO: see if we don't need this - basically Automapper appears to be trying to map the Id property on the child object anyway if it exists
                    var session = context.Options.CreateInstance<ISession>();
                    var entityPropertyInfo = (destinationEntity.Body as MemberExpression)?.Member as PropertyInfo;
                    var idFunc = selector.Compile();
                    var id = idFunc(source);
                    entityPropertyInfo?.SetValue(destination, id == null ? null : session.Get<TEntity>(id));
                });
        }
    }
}