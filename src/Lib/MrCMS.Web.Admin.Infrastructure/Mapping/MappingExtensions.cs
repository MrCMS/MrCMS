using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mapster;
using MrCMS.Entities;
using MrCMS.Mapping;

namespace MrCMS.Web.Admin.Infrastructure.Mapping
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Sets an entity reference on the destination by loading it from the current session using an id on the source
        /// (e.g. model.ParentId -> webpage.Parent).
        /// </summary>
        public static TypeAdapterSetter<TSource, TDestination> MapEntityLookup<TSource, TDestination, TEntity>(
            this TypeAdapterSetter<TSource, TDestination> setter,
            Expression<Func<TSource, int?>> selector,
            Expression<Func<TDestination, TEntity>> destinationEntity,
            Func<TSource, TDestination, bool> condition = null)
            where TEntity : SystemEntity
        {
            var entityProperty = (destinationEntity.Body as MemberExpression)?.Member as PropertyInfo
                                 ?? throw new ArgumentException("Destination must be a property", nameof(destinationEntity));
            var idFunc = selector.Compile();

            return setter
                .Ignore(entityProperty.Name)
                .AfterMapping((source, destination) =>
                {
                    if (condition != null && !condition(source, destination))
                        return;

                    var session = MapContext.Current.GetSession();
                    if (session == null)
                        return;

                    var id = idFunc(source);
                    entityProperty.SetValue(destination, id == null ? null : session.Get<TEntity>(id));
                });
        }

        /// <summary>
        /// Replaces the contents of an existing collection on the destination rather than swapping the instance,
        /// which NHibernate requires for mapped collections.
        /// </summary>
        public static void ReplaceContents<T>(this ISet<T> target, IEnumerable<T> values)
        {
            target.Clear();
            if (values == null)
                return;
            foreach (var value in values)
                target.Add(value);
        }
    }
}
