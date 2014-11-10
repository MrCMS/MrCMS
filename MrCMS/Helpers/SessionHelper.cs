using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Helpers
{
    public static class SessionHelper
    {
        public static ISession OpenFilteredSession(this ISessionFactory sessionFactory)
        {
            var session = new MrCMSSession(sessionFactory.OpenSession());
            session.EnableFilter("NotDeletedFilter");
            return session;
        }

        public static TResult Transact<TResult>(this ISession session, Func<ISession, TResult> func)
        {
            if (!session.Transaction.IsActive)
            {
                // Wrap in transaction
                TResult result;
                using (ITransaction tx = session.BeginTransaction())
                {
                    result = func.Invoke(session);
                    tx.Commit();
                }
                return result;
            }

            // Don't wrap;
            return func.Invoke(session);
        }

        public static void Transact(this ISession session, Action<ISession> action)
        {
            Transact(session, ses =>
            {
                action.Invoke(ses);
                return false;
            });
        }

        public static IPagedList<T> Paged<T>(this ISession session, QueryOver<T> query, int pageNumber,
            int? pageSize = null)
            where T : SystemEntity
        {
            int size = pageSize ?? MrCMSApplication.Get<SiteSettings>().DefaultPageSize;
            IEnumerable<T> values =
                query.GetExecutableQueryOver(session).Skip((pageNumber - 1)*size).Take(size).Cacheable().List<T>();

            var rowCount = query.GetExecutableQueryOver(session).ToRowCountQuery().SingleOrDefault<int>();

            return new StaticPagedList<T>(values, pageNumber, size, rowCount);
        }

        public static IPagedList<TResult> Paged<TResult>(this IQueryOver<TResult, TResult> queryBase, int pageNumber,
            int? pageSize = null)
            where TResult : SystemEntity
        {
            int size = pageSize ?? MrCMSApplication.Get<SiteSettings>().DefaultPageSize;
            IEnumerable<TResult> results = queryBase.Skip((pageNumber - 1)*size).Take(size).Cacheable().List();

            int rowCount = queryBase.Cacheable().RowCount();

            return new StaticPagedList<TResult>(results, pageNumber, size, rowCount);
        }

        public static IPagedList<TResult> Paged<TResult>(this IQueryable<TResult> queryable, int pageNumber,
            int? pageSize = null)
            where TResult : SystemEntity
        {
            int size = pageSize ?? MrCMSApplication.Get<SiteSettings>().DefaultPageSize;

            IQueryable<TResult> cacheable = queryable.Cacheable();

            return new PagedList<TResult>(cacheable, pageNumber, size);
        }

        public static bool Any<T>(this IQueryOver<T> query)
        {
            return query.RowCount() > 0;
        }
    }
}