using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Data
{
    public class QueryableRepository<T> : IQueryableRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public virtual IQueryable<T> Query()
        {
            IQueryable<T> dbSet = Context.Set<T>();

            if (typeof(T).IsImplementationOf(typeof(ICanSoftDelete)))
            {
                dbSet = dbSet.Where(x => EF.Property<bool>(x, nameof(ICanSoftDelete.IsDeleted)) == false);
            }

            return dbSet;
        }

        public IQueryable<TSubtype> Query<TSubtype>() where TSubtype : class, T
        {
            return Query().OfType<TSubtype>();
        }

        public IQueryable<T> Readonly()
        {
            return Query().AsNoTracking();
        }

        public IQueryable<TSubtype> Readonly<TSubtype>() where TSubtype : class, T
        {
            return Readonly().OfType<TSubtype>();
        }

        public QueryableRepository(IServiceProvider provider)
            : this(provider.GetRequiredService<IMrCmsContextResolver>().Resolve())
        {
        }

        internal QueryableRepository(DbContext context)
        {
            Context = context;
        }
    }
}