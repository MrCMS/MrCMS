using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Data
{
    public abstract class OnDataAdding
    {
        public abstract Task<IResult> OnAdding(IHaveId entity, DbContext context);
        public abstract Task<IResult> OnAdding(IEnumerable<IHaveId> entities, DbContext context);
        public static readonly Task<IResult> Success = Task.FromResult((IResult)new Successful());
    }
    public abstract class OnDataAdding<T> : OnDataAdding where T : class, IHaveId
    {
        public abstract Task<IResult> OnAdding(T entity, DbContext context);
        public sealed override Task<IResult> OnAdding(IHaveId entity, DbContext context)
        {
            return entity is T typed
                ? OnAdding(typed, context)
                : Task.FromResult((IResult)new Failure($"Entity is not of type {typeof(T).Name.BreakUpString()}"));
        }

        public virtual async Task<IResult> OnAdding(ICollection<T> entities, DbContext context)
        {
            var tasks = entities.Select(e => OnAdding(e, context));
            var results = new List<IResult>();
            foreach (var task in tasks)
                results.Add(await task);
            if (results.Any(x => !x.Success))
                return new Failure(results.Where(x => !x.Success).SelectMany(x => x.Messages));

            return await Success;
        }

        public sealed override Task<IResult> OnAdding(IEnumerable<IHaveId> entities, DbContext context)
        {
            var haveIds = entities as IList<IHaveId> ?? entities.ToList();
            var typed = haveIds.OfType<T>().ToList();
            return haveIds.Count == typed.Count
                ? OnAdding(typed, context)
                : Task.FromResult(
                    (IResult)new Failure($"All entities are not of type {typeof(T).Name.BreakUpString()}"));
        }
    }
}