using System.Threading;
using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.Notifications;

namespace MrCMS.Services.WebsiteManagement
{
    public class MoveWebpageBatchJobExecutor : BaseBatchJobExecutor<MoveWebpageBatchJob>
    {
        private readonly INotificationDisabler _notificationDisabler;
        private readonly IRepository<Webpage> _repository;

        public MoveWebpageBatchJobExecutor(IRepository<Webpage> repository, INotificationDisabler notificationDisabler)
        {
            _repository = repository;
            _notificationDisabler = notificationDisabler;
        }


        protected override async Task<BatchJobExecutionResult> OnExecuteAsync(MoveWebpageBatchJob batchJob,
            CancellationToken token)
        {
            using (_notificationDisabler.Disable())
            {
                var webpage = await _repository.Load(batchJob.WebpageId);
                if (webpage == null)
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);

                var parent = batchJob.NewParentId.HasValue ? await _repository.Load(batchJob.NewParentId.Value) : null;
                if (batchJob.NewParentId.HasValue && parent == null)
                    return BatchJobExecutionResult.Failure("Could not find the parent webpage with id " +
                                                           batchJob.NewParentId);

                webpage.Parent = parent;
                await _repository.Update(webpage, token);

                return BatchJobExecutionResult.Success();
            }
        }
    }
}