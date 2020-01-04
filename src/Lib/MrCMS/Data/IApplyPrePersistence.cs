using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IApplyPrePersistence<in T> where T : class
    {
        Task<IResult<TSubtype>> OnAdding<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T;

        Task<IResult<ICollection<TSubtype>>> OnAdding<TSubtype>(ICollection<TSubtype> entities, DbContext context)
            where TSubtype : class, T;

        Task<IResult<TSubtype>> OnUpdating<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T;

        Task<IResult<ICollection<TSubtype>>> OnUpdating<TSubtype>(ICollection<TSubtype> entities, DbContext context)
            where TSubtype : class, T;

        Task<IResult<TSubtype>> OnDeleting<TSubtype>(TSubtype entity, DbContext context) where TSubtype : class, T;

        Task<IResult<ICollection<TSubtype>>> OnDeleting<TSubtype>(ICollection<TSubtype> entities, DbContext context)
            where TSubtype : class, T;
    }
}