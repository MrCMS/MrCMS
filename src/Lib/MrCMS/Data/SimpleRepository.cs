using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Data
{
    public class SimpleRepository<T> : IRepository<T> where T : SystemEntity
    {
        private readonly ISession _session;

        public SimpleRepository(ISession session)
        {
            _session = session;
        }

        public async Task<T> Get(int id)
        {
            return (await _session.GetAsync<T>(id)).Unproxy();
        }

        public async Task Add(T entity)
        {
            await _session.TransactAsync(session => session.SaveAsync(entity));
        }

        public async Task Delete(T entity)
        {
            await _session.TransactAsync(session => session.DeleteAsync(entity));
        }

        public IQueryable<T> Query()
        {
            return _session.Query<T>().WithOptions(options => options.SetCacheable(true));
        }

        public async Task TransactAsync(Func<IRepository<T>, Task> func)
        {
            await _session.TransactAsync(async session => { await func(this); });
        }

        public async Task Update(T entity)
        {
            await _session.TransactAsync(session => session.UpdateAsync(entity));
        }
    }
}