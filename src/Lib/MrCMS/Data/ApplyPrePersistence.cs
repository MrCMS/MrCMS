using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public class ApplyPrePersistence<T> : IApplyPrePersistence<T> where T : class, IHaveId
    {
        private readonly IGetPrePersistence _getPrePersistence;

        public ApplyPrePersistence(IGetPrePersistence getPrePersistence)
        {
            _getPrePersistence = getPrePersistence;
        }

        public async Task<IResult<TSubtype>> OnAdding<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var onAdding in _getPrePersistence.GetOnAdding<TSubtype>())
                results.Add(await onAdding.OnAdding(entity, context));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<TSubtype>(entity, failures.SelectMany(x => x.Messages));
            return new Successful<TSubtype>(entity);
        }

        public async Task<IResult<ICollection<TSubtype>>> OnAdding<TSubtype>(ICollection<TSubtype> entities, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var onAdding in _getPrePersistence.GetOnAdding<TSubtype>())
                results.Add(await onAdding.OnAdding(entities, context));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<ICollection<TSubtype>>(entities, failures.SelectMany(x => x.Messages));
            return new Successful<ICollection<TSubtype>>(entities);
        }

        public async Task<IResult<TSubtype>> OnUpdating<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var onUpdating in _getPrePersistence.GetOnUpdating<TSubtype>())
                results.Add(await onUpdating.OnUpdating(entity, context));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<TSubtype>(entity, failures.SelectMany(x => x.Messages));
            return new Successful<TSubtype>(entity);
        }

        public async Task<IResult<ICollection<TSubtype>>> OnUpdating<TSubtype>(ICollection<TSubtype> entities, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var onUpdating in _getPrePersistence.GetOnUpdating<TSubtype>())
                results.Add(await onUpdating.OnUpdating(entities, context));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<ICollection<TSubtype>>(entities, failures.SelectMany(x => x.Messages));
            return new Successful<ICollection<TSubtype>>(entities);
        }

        public async Task<IResult<TSubtype>> OnDeleting<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var onDeleting in _getPrePersistence.GetOnDeleting<TSubtype>())
                results.Add(await onDeleting.OnDeleting(entity, context));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<TSubtype>(entity, failures.SelectMany(x => x.Messages));
            return new Successful<TSubtype>(entity);
        }

        public async Task<IResult<ICollection<TSubtype>>> OnDeleting<TSubtype>(ICollection<TSubtype> entities, DbContext context) where TSubtype : class, T
        {
            var results = new List<IResult>();
            foreach (var onDeleting in _getPrePersistence.GetOnDeleting<TSubtype>())
                results.Add(await onDeleting.OnDeleting(entities, context));

            var failures = results.FindAll(x => !x.Success);
            if (failures.Any())
                return new Failure<ICollection<TSubtype>>(entities, failures.SelectMany(x => x.Messages));
            return new Successful<ICollection<TSubtype>>(entities);
        }
    }

    public interface IGetPrePersistence
    {
        IEnumerable<OnDataAdding> GetOnAdding<TSubtype>();
        IEnumerable<OnDataUpdating> GetOnUpdating<TSubtype>();
        IEnumerable<OnDataDeleting> GetOnDeleting<TSubtype>();
    }

}