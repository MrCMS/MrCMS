using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;
using MrCMS.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Criterion.Lambda;
using NHibernate.Transform;
using X.PagedList;
using ISession = NHibernate.ISession;

namespace MrCMS.Helpers
{
    public static class SessionHelper
    {
        public static int DefaultPageSize = 25;

        public static ISession OpenFilteredSession(this ISessionFactory sessionFactory,
            IServiceProvider serviceProvider)
        {
            var sessionBuilder = sessionFactory.WithOptions()
                .Interceptor(new MrCMSInterceptor(serviceProvider));
            var session = new MrCMSSession(sessionBuilder.OpenSession());
            session.EnableFilter("NotDeletedFilter");
            return session;
        }

        public static HttpContext GetContext(this ISession session)
        {
            return GetContext(session.GetSessionImplementation());
        }

        public static HttpContext GetContext(this ISessionImplementor session)
        {
            return (session.Interceptor as MrCMSInterceptor)?.Context;
        }

        public static T GetService<T>(this ISession session)
        {
            return session.GetSessionImplementation().GetService<T>();
        }

        public static T GetService<T>(this ISessionImplementor sessionImplementor)
        {
            return !(sessionImplementor.Interceptor is MrCMSInterceptor mrCMSInterceptor)
                ? default(T)
                : mrCMSInterceptor.ServiceProvider.GetRequiredService<T>();
        }


        public static Task TransactAsync(this ISession session, Func<ISession, Task> action)
        {
            return TransactAsync(session, (s, token) => action(s), CancellationToken.None);
        }

        public static Task TransactAsync(this ISession session, Func<ISession, CancellationToken, Task> action)
        {
            return TransactAsync(session, action, CancellationToken.None);
        }

        public static async Task<TResult> TransactAsync<TResult>(this ISession session,
            Func<ISession, CancellationToken, Task<TResult>> func, CancellationToken cancellationToken)
        {
            if (session.GetCurrentTransaction() != null)
                // Don't wrap;
                return await func.Invoke(session, cancellationToken);

            // Wrap in transaction
            using var tx = session.BeginTransaction();
            var result = await func.Invoke(session, cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return result;
        }

        public static async Task<TResult> TransactAsync<TResult>(this ISession session,
            Func<ISession, Task<TResult>> func)
        {
            if (session.GetCurrentTransaction() != null)
                // Don't wrap;
                return await func.Invoke(session);

            // Wrap in transaction
            using var tx = session.BeginTransaction();
            var result = await func.Invoke(session);
            await tx.CommitAsync();

            return result;
        }

        public static Task TransactAsync(this ISession session, Func<ISession, CancellationToken, Task> action,
            CancellationToken cancellationToken)
        {
            return TransactAsync(session, async (ses, token) =>
                {
                    await action.Invoke(ses, token);
                    return false;
                },
                cancellationToken);
        }


        public static async Task TransactAsync(this IStatelessSession session,
            Func<IStatelessSession, Task> func)
        {
            if (session.GetCurrentTransaction() != null)
            {
                // Don't wrap;
                await func.Invoke(session);
                return;
            }

            // Wrap in transaction
            using ITransaction tx = session.BeginTransaction();
            await func.Invoke(session);
            await tx.CommitAsync();
        }

        public static async Task<TResult> TransactAsync<TResult>(this IStatelessSession session,
            Func<IStatelessSession, Task<TResult>> func)
        {
            if (session.GetCurrentTransaction() != null)
                // Don't wrap;
                return await func.Invoke(session);

            // Wrap in transaction
            using ITransaction tx = session.BeginTransaction();
            var result = await func.Invoke(session);
            await tx.CommitAsync();

            return result;
        }

        public static async Task TransactAsync(this IStatelessSession session,
            Func<IStatelessSession, CancellationToken, Task> func, CancellationToken cancellationToken)
        {
            if (session.GetCurrentTransaction() != null)
            {
                // Don't wrap;
                await func.Invoke(session, cancellationToken);
                return;
            }

            // Wrap in transaction
            using ITransaction tx = session.BeginTransaction();
            await func.Invoke(session, cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }

        public static async Task<TResult> TransactAsync<TResult>(this IStatelessSession session,
            Func<IStatelessSession, CancellationToken, Task<TResult>> func, CancellationToken cancellationToken)
        {
            if (session.GetCurrentTransaction() != null)
                // Don't wrap;
                return await func.Invoke(session, cancellationToken);

            // Wrap in transaction
            using ITransaction tx = session.BeginTransaction();
            var result = await func.Invoke(session, cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return result;
        }


        public static IPagedList<T> Paged<T>(this ISession session, QueryOver<T> query, int pageNumber,
            int? pageSize = null)
        {
            var size = pageSize ?? DefaultPageSize;
            IEnumerable<T> values = query.GetExecutableQueryOver(session).Skip((pageNumber - 1) * size)
                .Take(size).Cacheable().List<T>();

            var rowCount = query.GetExecutableQueryOver(session).ToRowCountQuery().SingleOrDefault<int>();

            return new StaticPagedList<T>(values, pageNumber, size, rowCount);
        }
        public static async Task<IPagedList<T>> PagedAsync<T>(this ISession session, QueryOver<T> query, int pageNumber,
            int? pageSize = null)
        {
            var size = pageSize ?? DefaultPageSize;
            IEnumerable<T> values = await query.GetExecutableQueryOver(session).Skip((pageNumber - 1) * size)
                .Take(size).Cacheable().ListAsync<T>();

            var rowCount = await query.GetExecutableQueryOver(session).ToRowCountQuery().SingleOrDefaultAsync<int>();

            return new StaticPagedList<T>(values, pageNumber, size, rowCount);
        }

        public static IPagedList<TResult> Paged<TResult>(this IQueryOver<TResult, TResult> queryBase, int pageNumber,
            int? pageSize = null)
        {
            var size = pageSize ?? DefaultPageSize;
            IEnumerable<TResult> results =
                queryBase.Skip((pageNumber - 1) * size).Take(size).Cacheable().List();

            int rowCount = queryBase.Cacheable().RowCount();

            return new StaticPagedList<TResult>(results, pageNumber, size, rowCount);
        }

        public static async Task<IPagedList<TResult>> PagedAsync<TResult>(this IQueryOver<TResult, TResult> queryBase,
            int pageNumber,
            int? pageSize = null)
        {
            var size = pageSize ?? DefaultPageSize;
            IEnumerable<TResult> results = await
                queryBase.Skip((pageNumber - 1) * size).Take(size).Cacheable().ListAsync();

            int rowCount = await queryBase.Cacheable().RowCountAsync();

            return new StaticPagedList<TResult>(results, pageNumber, size, rowCount);
        }

        public static IPagedList<TResult> PagedMapped<TQuery, TResult>(this IQueryOver<TQuery, TQuery> queryBase,
            int pageNumber,
            Func<QueryOverProjectionBuilder<TQuery>, QueryOverProjectionBuilder<TQuery>> builder,
            int? pageSize = null)
        {
            var size = pageSize ?? DefaultPageSize;
            IEnumerable<TResult> results =
                queryBase
                    .SelectList(builder)
                    .TransformUsing(Transformers.AliasToBean<TResult>())
                    .Skip((pageNumber - 1) * size).Take(size).Cacheable().List<TResult>();

            int rowCount = queryBase.Cacheable().RowCount();

            return new StaticPagedList<TResult>(results, pageNumber, size, rowCount);
        }

        public static async Task<IPagedList<TResult>> PagedMappedAsync<TQuery, TResult>(
            this IQueryOver<TQuery, TQuery> queryBase,
            int pageNumber,
            Func<QueryOverProjectionBuilder<TQuery>, QueryOverProjectionBuilder<TQuery>> builder,
            int? pageSize = null)
        {
            var size = pageSize ?? DefaultPageSize;
            IEnumerable<TResult> results =
                await queryBase
                    .SelectList(builder)
                    .TransformUsing(Transformers.AliasToBean<TResult>())
                    .Skip((pageNumber - 1) * size).Take(size).Cacheable().ListAsync<TResult>();

            int rowCount = await queryBase.Cacheable().RowCountAsync();

            return new StaticPagedList<TResult>(results, pageNumber, size, rowCount);
        }

        public static IPagedList<TResult> Paged<TResult>(this IQueryable<TResult> queryable, int pageNumber,
            int? pageSize = null)
        {
            var size = pageSize ?? DefaultPageSize;
            IQueryable<TResult> cacheable = queryable.WithOptions(options => options.SetCacheable(true));

            return new PagedList<TResult>(cacheable, pageNumber, size);
        }

        public static async Task<IPagedList<TResult>> PagedAsync<TResult>(this IQueryable<TResult> queryable,
            int pageNumber, 
            int? pageSize = null)
        {
            var size = pageSize ?? DefaultPageSize;
            IQueryable<TResult> cacheable = queryable.WithOptions(options => options.SetCacheable(true));
            IEnumerable<TResult> results = await
                cacheable
                    .Skip((pageNumber - 1) * size).Take(size).ToListAsync();

            int rowCount = await cacheable.CountAsync();

            return new StaticPagedList<TResult>(results, pageNumber, size, rowCount);
        }

        public static bool Any<T>(this IQueryOver<T> query)
        {
            return query.RowCount() > 0;
        }

        public static async Task<bool> AnyAsync<T>(this IQueryOver<T> query)
        {
            return await query.RowCountAsync() > 0;
        }

        public static async Task<T> GetByGuidAsync<T>(this ISession session, Guid guid) where T : SystemEntity
        {
            return await session.Query<T>().Where(f => f.Guid == guid).WithOptions(options => options.SetCacheable(true)).FirstOrDefaultAsync();
        }
    }
}