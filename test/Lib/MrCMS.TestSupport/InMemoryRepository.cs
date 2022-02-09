using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities;

namespace MrCMS.TestSupport
{
    public class InMemoryRepository<T> : IRepository<T> where T : SystemEntity
    {
        private readonly Dictionary<int, T> _store = new Dictionary<int, T>();
        private int _currentId = 1;

        public Task<T> Get(int id)
        {
            return Task.FromResult(_store.ContainsKey(id) ? _store[id] : null);
        }

        public Task Add(T entity)
        {
            entity.Id = _currentId++;
            _store[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task Delete(T entity)
        {
            _store.Remove(entity.Id);
            return Task.CompletedTask;
        }

        public async Task TransactAsync(Func<IRepository<T>, Task> func)
        {
            await func(this);
        }

        public IQueryable<T> Query()
        {
            return _store.Values.AsQueryable();
        }

        public Task Update(T entity)
        {
            if (!_store.ContainsKey(entity.Id))
                throw new Exception($"#{entity.Id} does not exist");
            _store[entity.Id] = entity;
            return Task.CompletedTask;
        }
    }
}