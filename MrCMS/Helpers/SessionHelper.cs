using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Paging;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Helpers
{
    public static class SessionHelper
    {
        public static ISession OpenFilteredSession(this ISessionFactory sessionFactory)
        {
            var session = sessionFactory.OpenSession();
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

        public static IPagedList<T> Paged<T>(this ISession session, QueryOver<T> query, int pageNumber, int pageSize)
            where T : SystemEntity
        {
            IEnumerable<T> values =
                       query.GetExecutableQueryOver(session)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .Cacheable()
                            .List<T>();

            var rowCount = query.GetExecutableQueryOver(session).ToRowCountQuery().SingleOrDefault<int>();

            return new StaticPagedList<T>(values, pageNumber, pageSize, rowCount);
        }

        public static IPagedList<TResult> Paged<TResult>(this IQueryOver<TResult, TResult> queryBase, int pageNumber, int pageSize)
            where TResult : SystemEntity
        {
            IEnumerable<TResult> results =
                queryBase.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                    .Cacheable().List();

            int rowCount = queryBase.Cacheable().RowCount();

            return new StaticPagedList<TResult>(results, pageNumber, pageSize, rowCount);
        }

        public static bool Any<T>(this IQueryOver<T> query)
        {
            return query.RowCount() > 0;
        }
    }

    public enum OrderType
    {
        Descending,
        Ascending
    }
}