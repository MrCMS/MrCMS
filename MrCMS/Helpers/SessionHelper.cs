using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MrCMS.Entities;
using MrCMS.Paging;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;

namespace MrCMS.Helpers
{
    public static class SessionHelper
    {
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


        public static QueryOver<T, T> SetOrder<T>(this QueryOver<T, T> query, IProjection projection,
                                                  OrderType orderType) where T : SystemEntity
        {
            if (projection == null) return query;

            QueryOverOrderBuilder<T, T> queryOverOrderBuilder = query.OrderBy(projection);
            switch (orderType)
            {
                case OrderType.Ascending:
                    return queryOverOrderBuilder.Asc;
                case OrderType.Descending:
                    return queryOverOrderBuilder.Desc;
                default:
                    throw new ArgumentOutOfRangeException("orderType");
            }
        }

        public static IQueryOver<T, T> SetFilters<T>(this IQueryOver<T, T> query,
                                                     IEnumerable<Expression<Func<T, bool>>> filters)
            where T : SystemEntity
        {
            return filters.Aggregate(query, (current, filter) => current.Where(filter));
        }

        public static IPagedList<T2> Paged<T, T2>(this ISession session, QueryOver<T> query,
                                                  Func<IEnumerable<T>, IEnumerable<T2>> transform, int pageNumber,
                                                  int pageSize) where T : SystemEntity
        {
            IEnumerable<T> values =
                query.GetExecutableQueryOver(session)
                     .Skip((pageNumber - 1)*pageSize)
                     .Take(pageSize)
                     .Cacheable()
                     .List<T>();

            var rowCount = query.GetExecutableQueryOver(session).ToRowCountQuery().SingleOrDefault<int>();

            return new StaticPagedList<T2>(transform(values), pageNumber, pageSize, rowCount);
        }

        public static IPagedList<T> Paged<T>(this ISession session, QueryOver<T> query, int pageNumber, int pageSize)
            where T : SystemEntity
        {
            return Paged(session, query, enumerable => enumerable, pageNumber, pageSize);
        }

        public static IPagedList<TResult> Paged<TBase, TResult>(this ISession session, int pageNumber, int pageSize,
                                                                Func<IEnumerable<TBase>, IEnumerable<TResult>> transform,
                                                                IEnumerable<Expression<Func<TBase, bool>>> restrictions
                                                                    = null, Expression<Func<TBase, object>> order = null,
                                                                OrderType orderType = OrderType.Ascending)
            where TBase : SystemEntity
        {
            IEnumerable<TBase> results =
                GetBasePagedQuery(session, restrictions, order, orderType).Skip((pageNumber - 1) * pageSize).Take(pageSize)
                    .Cacheable().Future();

            int rowCount = GetBasePagedQuery(session, restrictions, order, orderType).Cacheable().RowCount();

            return new StaticPagedList<TResult>(transform(results), pageNumber, pageSize, rowCount);
        }

        public static IPagedList<TResult> Paged<TResult>(this ISession session, int pageNumber, int pageSize,
                                                         IEnumerable<Expression<Func<TResult, bool>>> restrictions =
                                                             null, Expression<Func<TResult, object>> order = null,
                                                         OrderType orderType = OrderType.Ascending)
            where TResult : SystemEntity
        {
            IEnumerable<TResult> results =
                GetBasePagedQuery(session, restrictions, order, orderType).Skip((pageNumber - 1) * pageSize).Take(pageSize)
                    .Cacheable().Future();

            int rowCount = GetBasePagedQuery(session, restrictions, order, orderType).Cacheable().RowCount();

            return new StaticPagedList<TResult>(results, pageNumber, pageSize, rowCount);
        }

        public static IPagedList<TResult> Paged<TBase, TResult>(this IQueryOver<TBase, TBase> queryBase, int pageNumber, int pageSize,
                                                                Func<IEnumerable<TBase>, IEnumerable<TResult>> transform,
                                                                IEnumerable<Expression<Func<TBase, bool>>> restrictions
                                                                    = null, Expression<Func<TBase, object>> order = null,
                                                                OrderType orderType = OrderType.Ascending)
            where TBase : SystemEntity
        {
            IEnumerable<TBase> results =
                GetBasePagedQuery(queryBase, restrictions, order, orderType).Skip((pageNumber - 1) * pageSize).Take(pageSize)
                    .Cacheable().Future();

            int rowCount = GetBasePagedQuery(queryBase, restrictions, order, orderType).Cacheable().RowCount();

            return new StaticPagedList<TResult>(transform(results), pageNumber, pageSize, rowCount);
        }

        public static IPagedList<TResult> Paged<TResult>(this IQueryOver<TResult, TResult> queryBase, int pageNumber, int pageSize,
                                                         IEnumerable<Expression<Func<TResult, bool>>> restrictions =
                                                             null, Expression<Func<TResult, object>> order = null,
                                                         OrderType orderType = OrderType.Ascending)
            where TResult : SystemEntity
        {
            IEnumerable<TResult> results =
                GetBasePagedQuery(queryBase, restrictions, order, orderType).Skip((pageNumber - 1) * pageSize).Take(pageSize)
                    .Cacheable().List();

            int rowCount = GetBasePagedQuery(queryBase, restrictions, order, orderType).Cacheable().RowCount();

            return new StaticPagedList<TResult>(results, pageNumber, pageSize, rowCount);
        }

        private static IQueryOver<TResult, TResult> GetBasePagedQuery<TResult>(ISession session,
                                                                               IEnumerable
                                                                                   <Expression<Func<TResult, bool>>>
                                                                                   restrictions,
                                                                               Expression<Func<TResult, object>> order,
                                                                               OrderType orderType)
            where TResult : SystemEntity
        {
            IQueryOver<TResult, TResult> iQueryOver = session.QueryOver<TResult>();
            if (restrictions != null)
                iQueryOver = iQueryOver.SetFilters(restrictions);
            if (order != null)
            {
                IQueryOverOrderBuilder<TResult, TResult> temp = iQueryOver.OrderBy(order);
                switch (orderType)
                {
                    case OrderType.Ascending:
                        iQueryOver = temp.Asc;
                        break;
                    case OrderType.Descending:
                        iQueryOver = temp.Desc;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("orderType");
                }
            }
            return iQueryOver;
        }

        private static IQueryOver<TResult, TResult> GetBasePagedQuery<TResult>(this IQueryOver<TResult, TResult> queryBase,
                                                                               IEnumerable
                                                                                   <Expression<Func<TResult, bool>>>
                                                                                   restrictions,
                                                                               Expression<Func<TResult, object>> order,
                                                                               OrderType orderType)
            where TResult : SystemEntity
        {
            if (restrictions != null)
                queryBase = queryBase.SetFilters(restrictions);
            if (order != null)
            {
                IQueryOverOrderBuilder<TResult, TResult> temp = queryBase.OrderBy(order);
                switch (orderType)
                {
                    case OrderType.Ascending:
                        queryBase = temp.Asc;
                        break;
                    case OrderType.Descending:
                        queryBase = temp.Desc;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("orderType");
                }
            }
            return queryBase;
        }

        public static IPagedList<TResult> Paged<TResult>(this ISession session, int pageNumber, int pageSize,
                                                            ICriterion restrictions = null, Order order = null)
            where TResult : SystemEntity
        {
            ICriteria rowCountCriteria =
                GetCriteria<TResult>(session).SetRestrictionsAndOrder(restrictions, order)
                    .SetCacheable(true)
                    .SetProjection(Projections.RowCount());
            rowCountCriteria.ClearOrders();
            IFutureValue<int> rowCountResult = rowCountCriteria.FutureValue<Int32>();

            List<TResult> result =
                GetCriteria<TResult>(session).SetRestrictionsAndOrder(restrictions, order)
                    .SetCacheable(true)
                    .SetFirstResult(
                        (pageNumber - 1) * pageSize).SetMaxResults(pageSize).List<TResult>().ToList<TResult>();

            return new StaticPagedList<TResult>(result, pageNumber, pageSize, rowCountResult.Value);
        }

        private static ICriteria GetCriteria<T1>(ISession session)
            where T1 : SystemEntity
        {
            return
                session.CreateCriteria<T1>();
        }
    }

    public static class QueryOverHelper
    {
        public static QueryOver<T, T> SetFilters<T>(this QueryOver<T, T> query,
                                                    IEnumerable<Expression<Func<T, bool>>> filters) where T : SystemEntity
        {
            return filters.Aggregate(query, (current, filter) => current.Where(filter));
        }

        public static QueryOver<T, T> SetFilters<T>(this QueryOver<T, T> query,
                                                    IEnumerable<Expression<Func<bool>>> filters) where T : SystemEntity
        {
            return filters.Aggregate(query, (current, filter) => current.Where(filter));
        }
        public static QueryOver<T, T> SetFilter<T>(this QueryOver<T, T> query,
                                                    Expression<Func<bool>> filter) where T : SystemEntity
        {
            return query.Where(filter);
        }

        public static QueryOver<T, T> SetOrder<T>(this QueryOver<T, T> query, KeyValuePair<Expression<Func<T, object>>, OrderType> selector) where T : SystemEntity
        {
            QueryOverOrderBuilder<T, T> queryOverOrderBuilder = query.OrderBy(selector.Key);
            switch (selector.Value)
            {
                case OrderType.Ascending:
                    query = queryOverOrderBuilder.Asc;
                    break;
                case OrderType.Descending:
                    query = queryOverOrderBuilder.Desc;
                    break;
            }
            return query;
        }

        public static ICriteria SetRestrictionsAndOrder(this ICriteria criteria, ICriterion restrictions, Order order)
        {
            if (restrictions != null)
                criteria.Add(restrictions);
            if (order != null)
                criteria.AddOrder(order);

            return criteria;
        }

        public static QueryOver<T, T> SetFilter<T>(this QueryOver<T, T> query, Func<QueryOver<T, T>, QueryOver<T, T>> filter)
        {
            return filter.Invoke(query);
        }
    }
    public enum OrderType
    {
        Descending,
        Ascending
    }
}