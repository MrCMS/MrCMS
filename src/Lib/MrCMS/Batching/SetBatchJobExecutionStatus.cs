using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Batching
{
    public class SetBatchJobExecutionStatus : ISetBatchJobExecutionStatus
    {
        private readonly IServiceProvider _serviceProvider;

        public SetBatchJobExecutionStatus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Starting<T>(T entity) where T : SystemEntity, IHaveJobExecutionStatus
        {
            entity.Status = JobExecutionStatus.Executing;
            await _serviceProvider.GetRequiredService<IGlobalRepository<T>>().Update(entity);
        }

        public async Task Complete<T>(T entity, BatchJobExecutionResult result) where T : SystemEntity, IHaveJobExecutionStatus
        {
            entity.Status = result.Successful ? JobExecutionStatus.Succeeded : JobExecutionStatus.Failed;
            await _serviceProvider.GetRequiredService<IGlobalRepository<T>>().Update(entity);

        }
    }
}