using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Batching.Services
{
    public class BatchJobExecutionService : IBatchJobExecutionService
    {
        private readonly IKernel _kernel;

        public BatchJobExecutionService(IKernel kernel)
        {
            _kernel = kernel;
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
                    _executorTypeList[batchJobType] = executorTypes.First();
            }
        }

        public BatchJobExecutionResult Execute(BatchJob batchJob)
        {
            var type = batchJob.GetType();
            var hasExecutorType = _executorTypeList.ContainsKey(type);
            if (hasExecutorType)
            {
                var batchJobExecutor = _kernel.Get(_executorTypeList[type]) as IBatchJobExecutor;
                if (batchJobExecutor != null)
                    return batchJobExecutor.Execute(batchJob);
            }

            return _kernel.Get<DefaultBatchJobExecutor>().Execute(batchJob);
        }
    }
}