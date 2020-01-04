using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IDataReader
    {
        IQueryable<T> Readonly<T>() where T : class, IHaveId, IHaveSite;
        IQueryable<T> GlobalReadonly<T>() where T : class, IHaveId;

        Task<T> Get<T>(int id, CancellationToken token = default) where T : class, IHaveId, IHaveSite;
        Task<object> Get(Type type, int id, CancellationToken token = default);
        Task<T> GlobalGet<T>(int id, CancellationToken token = default) where T : class, IHaveId;
        Task<object> GlobalGet(Type type, int id, CancellationToken token = default);
    }
}