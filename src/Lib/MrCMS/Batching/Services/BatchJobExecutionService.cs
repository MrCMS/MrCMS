using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;

namespace MrCMS.Batching.Services
{
    public class BatchJobExecutionService : IBatchJobExecutionService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISetBatchJobExecutionStatus _setBatchJobExecutionStatus;
        private readonly ILogger<BatchExecutionService> _logger;

        public BatchJobExecutionService(IServiceProvider serviceProvider, ISetBatchJobExecutionStatus setBatchJobExecutionStatus, ILogger<BatchExecutionService> logger)
        {
            _serviceProvider = serviceProvider;
            _setBatchJobExecutionStatus = setBatchJobExecutionStatus;
            _logger = logger;
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

        public async Task<BatchJobExecutionResult> Execute(BatchJob batchJob, CancellationToken token)
        {
            if (batchJob == null)
                return BatchJobExecutionResult.Failure();
            var type = batchJob.GetType();
            var hasExecutorType = _executorTypeList.ContainsKey(type);
            if (!hasExecutorType ||
                !(_serviceProvider.GetService(_executorTypeList[type]) is IBatchJobExecutor batchJobExecutor))
            {
                batchJobExecutor = _serviceProvider.GetRequiredService<DefaultBatchJobExecutor>();
            }

            try
            {
                await _setBatchJobExecutionStatus.Starting(batchJob);
                var result = await batchJobExecutor.Execute(batchJob, token);
                await _setBatchJobExecutionStatus.Complete(batchJob, result);
                return result;
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                return BatchJobExecutionResult.Failure(exception.Message);
            }
        }
    }
}