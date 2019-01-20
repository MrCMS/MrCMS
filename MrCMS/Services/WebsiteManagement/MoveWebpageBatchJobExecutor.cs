using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using NHibernate;

namespace MrCMS.Services.WebsiteManagement
{
    public class MoveWebpageBatchJobExecutor : BaseBatchJobExecutor<MoveWebpageBatchJob>
    {
        private readonly ISession _session;

        public MoveWebpageBatchJobExecutor(ISession session,
            ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus) : base(setBatchJobJobExecutionStatus)
        {
            _session = session;
        }

        protected override BatchJobExecutionResult OnExecute(MoveWebpageBatchJob batchJob)
        {
            using (new NotificationDisabler())
            {
                var webpage = _session.Get<Webpage>(batchJob.WebpageId);
                if (webpage == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);
                }

                var parent = batchJob.NewParentId.HasValue ? _session.Get<Webpage>(batchJob.NewParentId) : null;
                if (batchJob.NewParentId.HasValue &&  parent == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the parent webpage with id " + batchJob.NewParentId);
                }

                _session.Transact(session =>
                {
                    webpage.Parent = parent;
                    session.Update(webpage);
                });

                return BatchJobExecutionResult.Success();
            }
        }

        protected override Task<BatchJobExecutionResult> OnExecuteAsync(MoveWebpageBatchJob batchJob)
        {
            throw new System.NotImplementedException();
        }
    }
}