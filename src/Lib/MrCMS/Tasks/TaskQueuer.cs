using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class TaskQueuer : ITaskQueuer
    {
        private readonly IGlobalRepository<QueuedTask> _globalRepository;
        private readonly IRepository<QueuedTask> _repository;

        public TaskQueuer(IGlobalRepository<QueuedTask> globalRepository, IRepository<QueuedTask> repository)
        {
            _globalRepository = globalRepository;
            _repository = repository;
        }

        public async Task<IList<QueuedTask>> GetPendingQueuedTasks()
        {
            return await _repository.Transact(async (repo, ct) =>
            {
                var queuedAt = DateTime.UtcNow;
                var queuedTasks = await
                    repo.Query()
                        .Where(task => task.Status == TaskExecutionStatus.Pending)
                        .ToListAsync(ct);

                foreach (var task in queuedTasks)
                {
                    task.Status = TaskExecutionStatus.AwaitingExecution;
                    task.QueuedAt = queuedAt;
                }
                await repo.UpdateRange(queuedTasks, ct);
                return queuedTasks;
            });
        }


        public async Task<IList<Site>> GetPendingQueuedTaskSites()
        {
            return await _globalRepository.Query()
                .Where(task => task.Status == TaskExecutionStatus.Pending)
                .Select(task => task.Site)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IList<QueuedTask>> GetPendingLuceneTasks()
        {
            return await _repository.Transact(async (repo, ct) =>
            {
                var queuedAt = DateTime.UtcNow;
                var queuedTasks = await
                    repo.Query()
                        .Where(task => task.Status == TaskExecutionStatus.Pending)
                        .Where(task => EF.Functions.Like(task.Type, $"{typeof(InsertIndicesTask<>).FullName}%") ||
                                       EF.Functions.Like(task.Type, $"{typeof(UpdateIndicesTask<>).FullName}%") ||
                                       EF.Functions.Like(task.Type, $"{typeof(DeleteIndicesTask<>).FullName}%"))
                        .ToListAsync(ct);

                foreach (var task in queuedTasks)
                {
                    task.Status = TaskExecutionStatus.AwaitingExecution;
                    task.QueuedAt = queuedAt;
                }
                await repo.UpdateRange(queuedTasks, ct);
                return queuedTasks;
            });
        }
    }
}