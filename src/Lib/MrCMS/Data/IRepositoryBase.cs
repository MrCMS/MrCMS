using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Common;
using MrCMS.Entities;
using MrCMS.Models;

namespace MrCMS.Data
{
    public interface IRepositoryBase<T> : IQueryableRepository<T> where T : class//, IHaveId
    {

        ///// <summary>
        /////     Gets the data tracked (for data manipulation)
        ///// </summary>
        ///// <param name="id">Id of the requested object</param>
        ///// <param name="includes">related entities to load</param>
        ///// <returns></returns>
        //Task<T> Load(int id, params Expression<Func<T, object>>[] includes);

        ///// <summary>
        /////     Gets the data tracked (for data manipulation)
        ///// </summary>
        ///// <param name="id">Id of the requested object</param>
        ///// <param name="includes">related entities to load</param>
        ///// <returns></returns>
        //Task<TSubtype> Load<TSubtype>(int id, params Expression<Func<TSubtype, object>>[] includes) where TSubtype : class, T;

        ///// <summary>
        /////     Gets the data untracked (for display)
        ///// </summary>
        ///// <param name="id">Id of the requested object</param>
        ///// <param name="includes">related entities to load</param>
        ///// <returns></returns>
        //Task<T> GetData(int id, params Expression<Func<T, object>>[] includes);

        ///// <summary>
        /////     Gets the data untracked (for display)
        ///// </summary>
        ///// <param name="id">Id of the requested object</param>
        ///// <param name="includes">related entities to load</param>
        ///// <returns></returns>
        //Task<TSubtype> GetData<TSubtype>(int id, params Expression<Func<TSubtype, object>>[] includes) where TSubtype : class, T;

        ///// <summary>
        /////     Gets the data tracked (for data manipulation)
        ///// </summary>
        ///// <param name="id">Id of the requested object</param>
        ///// <param name="includes">related entities to load</param>
        ///// <returns></returns>
        //T LoadSync(int id, params Expression<Func<T, object>>[] includes);

        ///// <summary>
        /////     Gets the data tracked (for data manipulation)
        ///// </summary>
        ///// <param name="id">Id of the requested object</param>
        ///// <param name="includes">related entities to load</param>
        ///// <returns></returns>
        //TSubtype LoadSync<TSubtype>(int id, params Expression<Func<TSubtype, object>>[] includes) where TSubtype : class, T;

        ///// <summary>
        /////     Gets the data untracked (for display)
        ///// </summary>
        ///// <param name="id">Id of the requested object</param>
        ///// <param name="includes">related entities to load</param>
        ///// <returns></returns>
        //T GetDataSync(int id, params Expression<Func<T, object>>[] includes);

        ///// <summary>
        /////     Gets the data untracked (for display)
        ///// </summary>
        ///// <param name="id">Id of the requested object</param>
        ///// <param name="includes">related entities to load</param>
        ///// <returns></returns>
        //TSubtype GetDataSync<TSubtype>(int id, params Expression<Func<TSubtype, object>>[] includes) where TSubtype : class, T;

        Task<IResult<TSubtype>> Add<TSubtype>(TSubtype entity, CancellationToken token = default) where TSubtype : class, T;

        Task<IResult<ICollection<TSubtype>>> AddRange<TSubtype>(ICollection<TSubtype> entities, CancellationToken token = default)
            where TSubtype : class, T;

        Task<IResult<TSubtype>> Update<TSubtype>(TSubtype entity, CancellationToken token = default) where TSubtype : class, T;

        Task<IResult<ICollection<TSubtype>>> UpdateRange<TSubtype>(ICollection<TSubtype> entities, CancellationToken token = default)
            where TSubtype : class, T;

        Task<IResult> Delete(T entity, CancellationToken token = default);
        Task<IResult> DeleteRange(ICollection<T> entities, CancellationToken token = default);
    }

    public interface IRepositoryBase<T, out TRepo> : IRepositoryBase<T> where TRepo : IRepositoryBase<T> where T : class/*, IHaveId*/
    {
        Task<TResult> Transact<TResult>(Func<TRepo, CancellationToken, Task<TResult>> func, CancellationToken token = default);
        Task Transact(Func<TRepo, CancellationToken, Task> action, CancellationToken token = default);
    }
}