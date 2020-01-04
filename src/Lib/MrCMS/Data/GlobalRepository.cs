using System;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public class GlobalRepository<T> : RepositoryBase<T, IGlobalRepository<T>>, IGlobalRepository<T>
        where T : class, IHaveId
    {
        public GlobalRepository(IServiceProvider provider) : base(provider)
        {
        }

        public GlobalRepository(DbContext context, ITryGetResult @try, ICheckInTransaction checkInTransaction,
            IGetChangesFromContext getChangesFromContext, IHandleDataChanges handleDataChanges,
            ICheckAllValidators<T> checkAllValidators, IApplyPrePersistence<T> applyPrePersistence) : base(
            context, @try, checkInTransaction, getChangesFromContext, handleDataChanges, checkAllValidators,
            applyPrePersistence)
        {
        }

        protected override IGlobalRepository<T> RepoInstance => this;
    }
}