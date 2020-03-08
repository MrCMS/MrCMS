using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Common;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Data
{
    public abstract class RepositoryBase<T, TRepo> : QueryableRepository<T>, IRepositoryBase<T, TRepo>
        where T : class where TRepo : IRepositoryBase<T>
    {
        private readonly ITryGetResult _try;
        private readonly ICheckInTransaction _checkInTransaction;
        private readonly IGetChangesFromContext _getChangesFromContext;
        private readonly IHandleDataChanges _handleDataChanges;
        private readonly ICheckAllValidators<T> _checkAllValidators;
        private readonly IApplyPrePersistence<T> _applyPrePersistence;

        protected RepositoryBase(IServiceProvider provider)
            : this(provider.GetRequiredService<IMrCmsContextResolver>().Resolve(),
                provider.GetRequiredService<ITryGetResult>(),
                provider.GetRequiredService<ICheckInTransaction>(),
                provider.GetRequiredService<IGetChangesFromContext>(),
                provider.GetRequiredService<IHandleDataChanges>(),
                provider.GetRequiredService<ICheckAllValidators<T>>(),
                provider.GetRequiredService<IApplyPrePersistence<T>>()
            )
        {
        }

        internal RepositoryBase(DbContext context,
            ITryGetResult @try,
            ICheckInTransaction checkInTransaction,
            IGetChangesFromContext getChangesFromContext,
            IHandleDataChanges handleDataChanges,
            ICheckAllValidators<T> checkAllValidators,
            IApplyPrePersistence<T> applyPrePersistence) : base(context)
        {
            _try = @try;
            _checkInTransaction = checkInTransaction;
            _getChangesFromContext = getChangesFromContext;
            _handleDataChanges = handleDataChanges;
            _checkAllValidators = checkAllValidators;
            _applyPrePersistence = applyPrePersistence;
        }


        protected virtual void BeforeAdd(T entity)
        {
            if (entity is IHaveSystemDates dates)
            {
                var now = DateTime.UtcNow;

                if (dates.CreatedOn == DateTime.MinValue)
                    dates.CreatedOn = now;

                if (dates.UpdatedOn == DateTime.MinValue)
                    dates.UpdatedOn = now;
            }
        }
        protected virtual void BeforeUpdate(T entity)
        {
            if (entity is IHaveSystemDates dates)
            {
                var now = DateTime.UtcNow;

                if (dates.UpdatedOn == DateTime.MinValue)
                    dates.UpdatedOn = now;
            }
        }

        public async Task<IResult<TSubtype>> Add<TSubtype>(TSubtype entity, CancellationToken token) where TSubtype : class, T
        {
            BeforeAdd(entity);

            var result = await _applyPrePersistence.OnAdding(entity, Context);
            if (!result.Success)
                return result;

            var validationResult = await _checkAllValidators.OnAdding(entity, Context);
            if (!validationResult.Success)
                return validationResult;

            return await _try.GetResultAsync(async () =>
            {
                await Context.AddAsync(entity, token);
                await TrySave(token);
                return entity;
            });
        }

        public async Task<IResult<ICollection<TSubtype>>> AddRange<TSubtype>(ICollection<TSubtype> entities, CancellationToken token)
            where TSubtype : class, T
        {
            foreach (var entity in entities)
                BeforeAdd(entity);

            var result = await _applyPrePersistence.OnAdding(entities, Context);
            if (!result.Success)
                return result;

            var validationResult = await _checkAllValidators.OnAdding(entities, Context);
            if (!validationResult.Success)
                return validationResult;

            return await _try.GetResultAsync(async () =>
            {
                await Context.AddRangeAsync(entities, token);
                await TrySave(token);

                return entities;
            });
        }

        public async Task<IResult<TSubtype>> Update<TSubtype>(TSubtype entity, CancellationToken token)
            where TSubtype : class, T
        {
            BeforeUpdate(entity);

            var result = await _applyPrePersistence.OnUpdating(entity, Context);
            if (!result.Success)
                return result;

            var validationResult = await _checkAllValidators.OnUpdating(entity, Context);
            if (!validationResult.Success)
                return validationResult;

            return await _try.GetResultAsync(async () =>
            {
                Context.Update(entity);
                await TrySave(token);
                return entity;
            });
        }

        public async Task<IResult<ICollection<TSubtype>>> UpdateRange<TSubtype>(ICollection<TSubtype> entities, CancellationToken token)
            where TSubtype : class, T
        {
            foreach (var entity in entities)
                BeforeUpdate(entity);

            var validationResult = await _applyPrePersistence.OnUpdating(entities, Context);
            if (!validationResult.Success)
                return validationResult;

            validationResult = await _checkAllValidators.OnUpdating(entities, Context);
            if (!validationResult.Success)
                return validationResult;

            return await _try.GetResultAsync(async () =>
            {
                Context.UpdateRange(entities);
                await TrySave(token);
                return entities;
            });
        }

        public async Task<IResult> Delete(T entity, CancellationToken token)
        {
            var validationResult = await _applyPrePersistence.OnDeleting(entity, Context);
            if (!validationResult.Success)
                return validationResult;

            validationResult = await _checkAllValidators.OnDeleting(entity, Context);
            if (!validationResult.Success)
                return validationResult;

            return await _try.GetResultAsync(async () =>
            {
                HandleDelete(entity);
                await TrySave(token);
            });
        }

        private void HandleDelete(T entity)
        {
            if (entity is ICanSoftDelete softDelete)
            {
                softDelete.IsDeleted = true;
                Context.Update(entity);
            }
            else
                Context.Remove(entity);
        }

        public async Task<IResult> DeleteRange(ICollection<T> entities, CancellationToken token)
        {
            var validationResult = await _applyPrePersistence.OnDeleting(entities, Context);
            if (!validationResult.Success)
                return validationResult;

            validationResult = await _checkAllValidators.OnDeleting(entities, Context);
            if (!validationResult.Success)
                return validationResult;

            return await _try.GetResultAsync(async () =>
            {
                entities.ForEach(HandleDelete);
                await TrySave(token);
            });
        }

        private async Task TrySave(CancellationToken token)
        {
            if (!IsInTransaction())
            {
                var data = await SaveChanges(token);

                await _handleDataChanges.HandleChanges(data);
            }
        }

        private async Task<ContextChangeData> SaveChanges(CancellationToken token)
        {
            try
            {
                var changes = _getChangesFromContext.GetChanges<T>(Context);
                await Context.SaveChangesAsync(token);
                return changes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsInTransaction()
        {
            return _checkInTransaction.IsInTransaction(Context);
        }

        protected abstract TRepo RepoInstance { get; }

        public async Task<TResult> Transact<TResult>(Func<TRepo, CancellationToken, Task<TResult>> func, CancellationToken token)
        {
            if (IsInTransaction())
            {
                return await func(RepoInstance, token);
            }
            await using var transaction = await Context.Database.BeginTransactionAsync(token);
            try
            {
                var result = await func(RepoInstance, token);
                var data = await SaveChanges(token);
                await transaction.CommitAsync(token);
                await _handleDataChanges.HandleChanges(data);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(token);
                throw;
            }
        }

        public async Task Transact(Func<TRepo, CancellationToken, Task> action, CancellationToken token)
        {
            await Transact(async (repo, ct) =>
            {
                await action(repo, ct);
                return true;
            }, token);
        }
    }
}