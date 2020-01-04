using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Data
{
    public abstract class OnDataDeleting
    {
        public abstract Task<IResult> OnDeleting(IHaveId entity, DbContext dbContext);
        public abstract Task<IResult> OnDeleting(IEnumerable<IHaveId> entities, DbContext dbContext);
        public static readonly Task<IResult> Success = Task.FromResult((IResult)new Successful());
    }
    public abstract class OnDataDeleting<T> : OnDataDeleting where T : class, IHaveId
    {
        public abstract Task<IResult> OnDeleting(T entity, DbContext dbContext);
        public override Task<IResult> OnDeleting(IHaveId entity, DbContext dbContext)
        {
            return entity is T typed
                ? OnDeleting(typed, dbContext)
                : Task.FromResult((IResult)new Failure($"Entity is not of type {typeof(T).Name.BreakUpString()}"));
        }

        public virtual async Task<IResult> OnDeleting(ICollection<T> entities, DbContext dbContext)
        {
            var tasks = entities.Select(e => OnDeleting(e, dbContext));
            var results = new List<IResult>();
            foreach (var task in tasks)
                results.Add(await task);
            if (results.Any(x => !x.Success))
                return new Failure(results.Where(x => !x.Success).SelectMany(x => x.Messages));

            return await Success;
        }

        public override Task<IResult> OnDeleting(IEnumerable<IHaveId> entities, DbContext dbContext)
        {
            var haveIds = entities as IList<IHaveId> ?? entities.ToList();
            var typed = haveIds.OfType<T>().ToList();
            return haveIds.Count == typed.Count
                ? OnDeleting(typed, dbContext)
                : Task.FromResult(
                    (IResult)new Failure($"All entities are not of type {typeof(T).Name.BreakUpString()}"));
        }
    }
}