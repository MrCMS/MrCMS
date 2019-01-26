using System;
using System.Linq;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IRepository<T> where T : SystemEntity
    {
        IQueryable<T> Query();
        T Get(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        void Transact(Action<IRepository<T>> action);
    }
}