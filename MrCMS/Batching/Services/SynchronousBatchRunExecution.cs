using System;
using System.Diagnostics;
using MrCMS.Batching.Entities;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class SynchronousBatchRunExecution : ISynchronousBatchRunExecution
    {
        private readonly IExecuteNextBatchJob _executeNextBatchJob;

        public SynchronousBatchRunExecution(IExecuteNextBatchJob executeNextBatchJob)
        {
            _executeNextBatchJob = executeNextBatchJob;
        }

        public void Execute(BatchRun run)
        {
            while (_executeNextBatchJob.Execute(run))
            {
                
            }
        }
    }
}