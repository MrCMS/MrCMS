using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Data
{
    public abstract class OnDataUpdating
    {
        public abstract Task<IResult> OnUpdating(object entity, DbContext context);
        public abstract Task<IResult> OnUpdating(IEnumerable<object> entities, DbContext context);
        public static readonly Task<IResult> Success = Task.FromResult((IResult)new Successful());
    }
    public abstract class OnDataUpdating<T> : OnDataUpdating where T: class
    {
        public abstract Task<IResult> OnUpdating(T entity, DbContext context);
        public sealed override Task<IResult> OnUpdating(object entity, DbContext context)
        {
            return entity is T typed
                ? OnUpdating(typed, context)
                : Task.FromResult((IResult)new Failure($"Entity is not of type {typeof(T).Name.BreakUpString()}"));
        }

        public virtual async Task<IResult> OnUpdating(ICollection<T> entities, DbContext context)
        {
            var tasks = entities.Select(entity=> OnUpdating(entity, context));
            var results = new List<IResult>();
            foreach (var task in tasks)
                results.Add(await task);
            if (results.Any(x => !x.Success))
                return new Failure(results.Where(x => !x.Success).SelectMany(x => x.Messages));

            return await Success;
        }

        public sealed override Task<IResult> OnUpdating(IEnumerable<object> entities, DbContext context)
        {
            var haveIds = entities as IList<object> ?? entities.ToList();
            var typed = haveIds.OfType<T>().ToList();
            return haveIds.Count == typed.Count
                ? OnUpdating(typed, context)
                : Task.FromResult(
                    (IResult)new Failure($"All entities are not of type {typeof(T).Name.BreakUpString()}"));
        }
    }
}