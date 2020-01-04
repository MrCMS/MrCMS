using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Data
{
    public class QueryableRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public virtual IQueryable<T> Query()
        {
            return Context.Set<T>();
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
        protected QueryableRepository(IServiceProvider provider)
            : this(provider.GetRequiredService<IMrCmsContextResolver>().Resolve())
        {
        }

        internal QueryableRepository(DbContext context)
        {
            Context = context;
        }
    }
}