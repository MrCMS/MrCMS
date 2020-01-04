using System;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;

namespace MrCMS.Data
{
    public class JoinTableRepository<T> : RepositoryBase<T, IJoinTableRepository<T>>, IJoinTableRepository<T>
        where T : class, IJoinTable
    {
        public JoinTableRepository(IServiceProvider provider) : base(provider)
        {
        }

        public JoinTableRepository(DbContext context, ITryGetResult @try, ICheckInTransaction checkInTransaction,
            IGetChangesFromContext getChangesFromContext, IHandleDataChanges handleDataChanges,
            ICheckAllValidators<T> checkAllValidators, IApplyPrePersistence<T> applyPrePersistence) : base(
            context, @try, checkInTransaction, getChangesFromContext, handleDataChanges, checkAllValidators,
            applyPrePersistence)
        {
        }

        protected override IJoinTableRepository<T> RepoInstance => this;
    }
}