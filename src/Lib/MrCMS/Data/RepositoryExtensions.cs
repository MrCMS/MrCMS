using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public static class RepositoryExtensions
    {
        /// <summary>
        ///     Gets the data tracked (for data manipulation)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id">Id of the requested object</param>
        /// <param name="includes">related entities to load</param>
        /// <returns></returns>
        public static Task<T> Load<T>(this IRepositoryBase<T> repo, int id, CancellationToken token = default, params Expression<Func<T, object>>[] includes) where T : class, IHaveId
        {
            return Load<T, T>(repo, id, token, includes);
        }

        /// <summary>
        ///     Gets the data tracked (for data manipulation)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id">Id of the requested object</param>
        /// <param name="includes">related entities to load</param>
        /// <returns></returns>
        public static Task<TSubtype> Load<T, TSubtype>(this IRepositoryBase<T> repo, int id, CancellationToken token = default, params Expression<Func<TSubtype, object>>[] includes) where T : class, IHaveId
            where TSubtype : class, T
        {
            return BaseLoadQuery(repo, includes).FirstOrDefaultAsync(x => x.Id == id, token);
        }
        /// <summary>
        ///     Gets the data tracked (for data manipulation)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id">Id of the requested object</param>
        /// <param name="includes">related entities to load</param>
        /// <returns></returns>
        public static T LoadSync<T>(this IRepositoryBase<T> repo, int id, params Expression<Func<T, object>>[] includes) where T : class, IHaveId
        {
            return LoadSync<T, T>(repo, id, includes);
        }

        /// <summary>
        ///     Gets the data tracked (for data manipulation)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id">Id of the requested object</param>
        /// <param name="includes">related entities to load</param>
        /// <returns></returns>
        public static TSubtype LoadSync<T, TSubtype>(this IRepositoryBase<T> repo, int id, params Expression<Func<TSubtype, object>>[] includes) where T : class, IHaveId
            where TSubtype : class, T
        {
            return BaseLoadQuery(repo, includes).FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        ///     Gets the data untracked (for display)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id">Id of the requested object</param>
        /// <param name="includes">related entities to load</param>
        /// <returns></returns>
        public static Task<T> GetData<T>(this IRepositoryBase<T> repo, int id, CancellationToken token = default, params Expression<Func<T, object>>[] includes) where T : class, IHaveId
        {
            return GetData<T, T>(repo, id, token, includes);
        }

        /// <summary>
        ///     Gets the data untracked (for display)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id">Id of the requested object</param>
        /// <param name="includes">related entities to load</param>
        /// <returns></returns>
        public static T GetDataSync<T>(this IRepositoryBase<T> repo, int id, params Expression<Func<T, object>>[] includes) where T : class, IHaveId
        {
            return GetDataSync<T, T>(repo, id, includes);
        }

        /// <summary>
        ///     Gets the data untracked (for display)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id">Id of the requested object</param>
        /// <param name="includes">related entities to load</param>
        /// <returns></returns>
        public static Task<TSubtype> GetData<T, TSubtype>(this IRepositoryBase<T> repo, int id, CancellationToken token = default, params Expression<Func<TSubtype, object>>[] includes) where T : class, IHaveId
            where TSubtype : class, T
        {
            return BaseGetDataQuery(repo, includes).FirstOrDefaultAsync(x => x.Id == id, token);
        }

        /// <summary>
        ///     Gets the data untracked (for display)
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id">Id of the requested object</param>
        /// <param name="includes">related entities to load</param>
        /// <returns></returns>
        public static TSubtype GetDataSync<T, TSubtype>(this IRepositoryBase<T> repo, int id, params Expression<Func<TSubtype, object>>[] includes) where T : class, IHaveId
            where TSubtype : class, T
        {
            return BaseGetDataQuery(repo, includes).FirstOrDefault(x => x.Id == id);
        }

        private static IQueryable<TSubType> BaseLoadQuery<T, TSubType>(
            IRepositoryBase<T> repo,
            params Expression<Func<TSubType, object>>[] includes) where TSubType : class, T where T : class
        {
            var queryable = repo.Query<TSubType>();
            if (includes != null)
                foreach (var include in includes)
                    queryable = queryable.Include(include);
            return queryable;
        }
        private static IQueryable<TSubType> BaseGetDataQuery<T, TSubType>(
            IRepositoryBase<T> repo,
            params Expression<Func<TSubType, object>>[] includes) where TSubType : class, T where T : class
        {
            var queryable = repo.Readonly<TSubType>();
            if (includes != null)
                foreach (var include in includes)
                    queryable = queryable.Include(include);
            return queryable;
        }

    }
}