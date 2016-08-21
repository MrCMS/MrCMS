using System;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Data
{
    public interface IRepository<T> where T : SystemEntity
    {
        IQueryable<T> Query();
        T Get(int id);
        TSubclass Get<TSubclass>(int id) where TSubclass : T;
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteAll();

        void Transact(Action<IRepository<T>> action);
        IDisposable DisableSiteFilter();
    }

    public class SimpleRepository<T> : IRepository<T> where T : SystemEntity
    {
        private readonly ISession _session;

        public SimpleRepository(ISession session)
        {
            _session = session;
        }

        public T Get(int id)
        {
            return _session.Get<T>(id);
        }

        public TSubclass Get<TSubclass>(int id) where TSubclass : T
        {
            return _session.Get<TSubclass>(id);
        }

        public void Add(T entity)
        {
            _session.Transact(session => session.Save(entity));
        }

        public void Delete(T entity)
        {
            _session.Transact(session => session.Delete(entity));
        }

        public void DeleteAll()
        {
            _session.CreateQuery($"delete {typeof(T).Name} entity").ExecuteUpdate();
        }

        public IQueryable<T> Query()
        {
            return _session.Query<T>().Cacheable();
        }

        public void Transact(Action<IRepository<T>> action)
        {
            _session.Transact(session =>
            {
                action(this);
            });
        }

        public IDisposable DisableSiteFilter()
        {
            return new SiteFilterDisabler(_session);
        }

        public void Update(T entity)
        {
            _session.Transact(session => session.Update(entity));
        }
    }
}