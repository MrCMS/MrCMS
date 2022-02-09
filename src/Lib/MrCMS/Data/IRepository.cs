using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IRepository<T> where T : SystemEntity
    {
        IQueryable<T> Query();
        Task<T> Get(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);

        Task TransactAsync(Func<IRepository<T>, Task> func);
    }
}