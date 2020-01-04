using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Common;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Data
{
    public abstract class EntityValidatorBase
    {
        public abstract Task<IResult> OnAdding(object entity);
        public abstract Task<IResult> OnAdding(IEnumerable<object> entities);
        public abstract Task<IResult> OnUpdating(object entity);
        public abstract Task<IResult> OnUpdating(IEnumerable<object> entities);
        public abstract Task<IResult> OnDeleting(object entity);
        public abstract Task<IResult> OnDeleting(IEnumerable<object> entities);
        public static readonly Task<IResult> Success = Task.FromResult((IResult)new Successful());
    }

    public abstract class EntityValidatorBase<T> : EntityValidatorBase where T : class
    {
        public virtual Task<IResult> OnAdding(T entity) => Success;
        public override Task<IResult> OnAdding(object entity)
        {
            return entity is T typed
                ? OnAdding(typed)
                : Task.FromResult((IResult)new Failure($"Entity is not of type {typeof(T).Name.BreakUpString()}"));
        }

        public virtual async Task<IResult> OnAdding(ICollection<T> entities)
        {
            var tasks = entities.Select(OnAdding);
            var results = new List<IResult>();
            foreach (var task in tasks)
                results.Add(await task);
            if (results.Any(x => !x.Success))
                return new Failure(results.Where(x => !x.Success).SelectMany(x => x.Messages));

            return await Success;
        }

        public override Task<IResult> OnAdding(IEnumerable<object> entities)
        {
            var haveIds = entities as IList<object> ?? entities.ToList();
            var typed = haveIds.OfType<T>().ToList();
            return haveIds.Count == typed.Count
                ? OnAdding(typed)
                : Task.FromResult(
                    (IResult)new Failure($"All entities are not of type {typeof(T).Name.BreakUpString()}"));
        }

        public virtual Task<IResult> OnUpdating(T entity) => Success;
        public override Task<IResult> OnUpdating(object entity)
        {
            return entity is T typed
                ? OnUpdating(typed)
                : Task.FromResult((IResult)new Failure($"Entity is not of type {typeof(T).Name.BreakUpString()}"));
        }

        public virtual async Task<IResult> OnUpdating(ICollection<T> entities)
        {
            var tasks = entities.Select(OnUpdating);
            var results = new List<IResult>();
            foreach (var task in tasks)
                results.Add(await task);
            if (results.Any(x => !x.Success))
                return new Failure(results.Where(x => !x.Success).SelectMany(x => x.Messages));

            return await Success;
        }

        public override Task<IResult> OnUpdating(IEnumerable<object> entities)
        {
            var haveIds = entities as IList<object> ?? entities.ToList();
            var typed = haveIds.OfType<T>().ToList();
            return haveIds.Count == typed.Count
                ? OnUpdating(typed)
                : Task.FromResult(
                    (IResult)new Failure($"All entities are not of type {typeof(T).Name.BreakUpString()}"));
        }

        public virtual Task<IResult> OnDeleting(T entity) => Success;
        public override Task<IResult> OnDeleting(object entity)
        {
            return entity is T typed
                ? OnDeleting(typed)
                : Task.FromResult((IResult)new Failure($"Entity is not of type {typeof(T).Name.BreakUpString()}"));
        }

        public virtual async Task<IResult> OnDeleting(ICollection<T> entities)
        {
            var tasks = entities.Select(OnDeleting);
            var results = new List<IResult>();
            foreach (var task in tasks)
                results.Add(await task);
            if (results.Any(x => !x.Success))
                return new Failure(results.Where(x => !x.Success).SelectMany(x => x.Messages));

            return await Success;
        }

        public override Task<IResult> OnDeleting(IEnumerable<object> entities)
        {
            var haveIds = entities as IList<object> ?? entities.ToList();
            var typed = haveIds.OfType<T>().ToList();
            return haveIds.Count == typed.Count
                ? OnDeleting(typed)
                : Task.FromResult(
                    (IResult)new Failure($"All entities are not of type {typeof(T).Name.BreakUpString()}"));
        }
    }
}