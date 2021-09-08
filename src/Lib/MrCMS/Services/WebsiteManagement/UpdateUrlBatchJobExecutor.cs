using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using ISession = NHibernate.ISession;

namespace MrCMS.Services.WebsiteManagement
{
    public class UpdateUrlBatchJobExecutor : BaseBatchJobExecutor<UpdateUrlBatchJob>
    {
        private readonly ISession _session;
        private readonly INotificationDisabler _notificationDisabler;

        public UpdateUrlBatchJobExecutor(ISession session, INotificationDisabler notificationDisabler)
        {
            _session = session;
            _notificationDisabler = notificationDisabler;
        }


        protected override async Task<BatchJobExecutionResult> OnExecuteAsync(UpdateUrlBatchJob batchJob)
        {
            using (_notificationDisabler.Disable())
            {
                var webpage = _session.Get<Webpage>(batchJob.WebpageId);
                if (webpage == null)
                {
                    return
                        BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);
                }

                await _session.TransactAsync(async session =>
                {
                    var urlHistories = webpage.Urls.ToList();
                    foreach (var webpageUrl in urlHistories)
                    {
                        if (!batchJob.NewUrl.Equals(webpageUrl.UrlSegment, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        webpage.Urls.Remove(webpageUrl);
                        await session.DeleteAsync(webpageUrl);
                    }

                    webpage.UrlSegment = batchJob.NewUrl;
                    await session.UpdateAsync(webpage);
                });

                return BatchJobExecutionResult.Success();
            }
        }
    }
}