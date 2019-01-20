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
    public class UpdateUrlBatchJobExecutor : BaseBatchJobExecutor<UpdateUrlBatchJob>
    {
        private readonly ISession _session;

        public UpdateUrlBatchJobExecutor(ISession session,
            ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus) : base(setBatchJobJobExecutionStatus)
        {
            _session = session;
        }

        protected override BatchJobExecutionResult OnExecute(UpdateUrlBatchJob batchJob)
        {
            using (new NotificationDisabler())
            {
                var webpage = _session.Get<Webpage>(batchJob.WebpageId);
                if (webpage == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);
                }

                _session.Transact(session =>
                {
                    var urlHistories = webpage.Urls.ToList();
                    foreach (var webpageUrl in urlHistories)
                    {
                        if (!batchJob.NewUrl.Equals(webpageUrl.UrlSegment, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        webpage.Urls.Remove(webpageUrl);
                        session.Delete(webpageUrl);
                    }

                    webpage.UrlSegment = batchJob.NewUrl;
                    session.Update(webpage);
                });

                return BatchJobExecutionResult.Success();
            }
        }

        protected override Task<BatchJobExecutionResult> OnExecuteAsync(UpdateUrlBatchJob batchJob)
        {
            throw new System.NotImplementedException();
        }
    }
}