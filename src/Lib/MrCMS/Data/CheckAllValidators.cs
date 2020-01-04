using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public class CheckAllValidators<T> : ICheckAllValidators<T> where T : class
    {
        private readonly IGetValidators _getValidators;

        public CheckAllValidators(IGetValidators getValidators)
        {
            _getValidators = getValidators;
        }

        public async Task<IResult<TSubtype>> OnAdding<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var validator in _getValidators.GetDatabaseValidators<TSubtype>())
                results.Add(await validator.OnAdding(entity, context));
            foreach (var validator in _getValidators.GetEntityValidators<TSubtype>())
                results.Add(await validator.OnAdding(entity));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<TSubtype>(entity, failures.SelectMany(x => x.Messages));
            return new Successful<TSubtype>(entity);
        }

        public async Task<IResult<ICollection<TSubtype>>> OnAdding<TSubtype>(ICollection<TSubtype> entities, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var validator in _getValidators.GetDatabaseValidators<TSubtype>())
                results.Add(await validator.OnAdding(entities, context));
            foreach (var validator in _getValidators.GetEntityValidators<TSubtype>())
                results.Add(await validator.OnAdding(entities));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<ICollection<TSubtype>>(entities, failures.SelectMany(x => x.Messages));
            return new Successful<ICollection<TSubtype>>(entities);
        }

        public async Task<IResult<TSubtype>> OnUpdating<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var validator in _getValidators.GetDatabaseValidators<TSubtype>())
                results.Add(await validator.OnUpdating(entity, context));
            foreach (var validator in _getValidators.GetEntityValidators<TSubtype>())
                results.Add(await validator.OnUpdating(entity));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<TSubtype>(entity, failures.SelectMany(x => x.Messages));
            return new Successful<TSubtype>(entity);
        }

        public async Task<IResult<ICollection<TSubtype>>> OnUpdating<TSubtype>(ICollection<TSubtype> entities, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var validator in _getValidators.GetDatabaseValidators<TSubtype>())
                results.Add(await validator.OnUpdating(entities, context));
            foreach (var validator in _getValidators.GetEntityValidators<TSubtype>())
                results.Add(await validator.OnUpdating(entities));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<ICollection<TSubtype>>(entities, failures.SelectMany(x => x.Messages));
            return new Successful<ICollection<TSubtype>>(entities);
        }

        public async Task<IResult<TSubtype>> OnDeleting<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var validator in _getValidators.GetDatabaseValidators<TSubtype>())
                results.Add(await validator.OnDeleting(entity, context));
            foreach (var validator in _getValidators.GetEntityValidators<TSubtype>())
                results.Add(await validator.OnDeleting(entity));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<TSubtype>(entity, failures.SelectMany(x => x.Messages));
            return new Successful<TSubtype>(entity);
        }

        public async Task<IResult<ICollection<TSubtype>>> OnDeleting<TSubtype>(ICollection<TSubtype> entities, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var validator in _getValidators.GetDatabaseValidators<TSubtype>())
                results.Add(await validator.OnDeleting(entities, context));
            foreach (var validator in _getValidators.GetEntityValidators<TSubtype>())
                results.Add(await validator.OnDeleting(entities));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<ICollection<TSubtype>>(entities, failures.SelectMany(x => x.Messages));
            return new Successful<ICollection<TSubtype>>(entities);
        }
    }
}