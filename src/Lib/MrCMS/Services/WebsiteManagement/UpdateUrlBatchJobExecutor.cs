using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Batching;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Services.WebsiteManagement
{
    public class UpdateUrlBatchJobExecutor : BaseBatchJobExecutor<UpdateUrlBatchJob>
    {
        private readonly IRepository<Webpage> _repository;
        private readonly IRepository<UrlHistory> _urlHistoryRepository;
        private readonly INotificationDisabler _notificationDisabler;

        public UpdateUrlBatchJobExecutor(
            IRepository<Webpage> repository,
            IRepository<UrlHistory> urlHistoryRepository,
            INotificationDisabler notificationDisabler)
        {
            _repository = repository;
            _urlHistoryRepository = urlHistoryRepository;
            _notificationDisabler = notificationDisabler;
        }

        protected override async Task<BatchJobExecutionResult> OnExecuteAsync(UpdateUrlBatchJob batchJob,
            CancellationToken token)
        {
            using (_notificationDisabler.Disable())
            {
                var webpage = await _repository.Load(batchJob.WebpageId);
                if (webpage == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);
                }

                await _repository.Transact(async (repo, ct) =>
                 {
                     var urlHistories = webpage.Urls.ToList();
                     foreach (var webpageUrl in urlHistories)
                     {
                         if (!batchJob.NewUrl.Equals(webpageUrl.UrlSegment, StringComparison.InvariantCultureIgnoreCase))
                             continue;

                         webpage.Urls.Remove(webpageUrl);
                         await _urlHistoryRepository.Delete(webpageUrl, ct);
                     }

                     webpage.UrlSegment = batchJob.NewUrl;
                     await repo.Update(webpage, ct);
                 }, token);

                return BatchJobExecutionResult.Success();
            }
        }
    }
}