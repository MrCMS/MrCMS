using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Batching;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using ISession = NHibernate.ISession;

namespace MrCMS.Services.WebsiteManagement
{
    public class MoveWebpageBatchJobExecutor : BaseBatchJobExecutor<MoveWebpageBatchJob>
    {
        private readonly ISession _session;
        private readonly INotificationDisabler _notificationDisabler;

        public MoveWebpageBatchJobExecutor(ISession session, INotificationDisabler notificationDisabler)
        {
            _session = session;
            _notificationDisabler = notificationDisabler;
        }

        protected override BatchJobExecutionResult OnExecute(MoveWebpageBatchJob batchJob)
        {
            using (_notificationDisabler.Disable())
            {
                var webpage = _session.Get<Webpage>(batchJob.WebpageId);
                if (webpage == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);
                }

                var parent = batchJob.NewParentId.HasValue ? _session.Get<Webpage>(batchJob.NewParentId) : null;
                if (batchJob.NewParentId.HasValue && parent == null)
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