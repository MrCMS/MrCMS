using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;

namespace MrCMS.Batching.Services
{
    public class BatchJobExecutionService : IBatchJobExecutionService
    {
        private readonly IServiceProvider _serviceProvider;

        public BatchJobExecutionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static readonly Dictionary<Type, Type> _executorTypeList = new Dictionary<Type, Type>();

        static BatchJobExecutionService()
        {
            var batchJobTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<BatchJob>();

            foreach (var batchJobType in batchJobTypes)
            {
                var type = typeof(BaseBatchJobExecutor<>).MakeGenericType(batchJobType);
                var executorTypes = TypeHelper.GetAllTypesAssignableFrom(type);
                if (executorTypes.Any())
                {
                    _executorTypeList[batchJobType] = executorTypes.First();
                }
            }
        }

        public async Task<BatchJobExecutionResult> Execute(BatchJob batchJob)
        {
            batchJob = batchJob.Unproxy();
            var type = batchJob.GetType();
            var hasExecutorType = _executorTypeList.ContainsKey(type);
            if (hasExecutorType)
            {
                if (_serviceProvider.GetService(_executorTypeList[type]) is IBatchJobExecutor batchJobExecutor)
                    return await batchJobExecutor.Execute(batchJob);
            }

            return await _serviceProvider.GetRequiredService<DefaultBatchJobExecutor>().Execute(batchJob);
        }
    }
}