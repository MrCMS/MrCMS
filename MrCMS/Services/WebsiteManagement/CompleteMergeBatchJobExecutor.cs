using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using NHibernate;

namespace MrCMS.Services.WebsiteManagement
{
    public class CompleteMergeBatchJobExecutor: BaseBatchJobExecutor<CompleteMergeBatchJob>
    {
        private readonly ISession _session;

        public CompleteMergeBatchJobExecutor(ISession session,
            ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus) : base(setBatchJobJobExecutionStatus)
        {
            _session = session;
        }


        protected override BatchJobExecutionResult OnExecute(CompleteMergeBatchJob batchJob)
        {
            using (new NotificationDisabler())
            {
                var webpage = _session.Get<Webpage>(batchJob.WebpageId);
                if (webpage == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);
                }
                var mergeInto = _session.Get<Webpage>(batchJob.MergedIntoId);
                if (mergeInto == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.MergedIntoId);
                }

                _session.Transact(session =>
                {
                    var urlSegment = webpage.UrlSegment;
                    session.Delete(webpage);
                    var urlHistory = new UrlHistory
                    {
                        UrlSegment = urlSegment,
                        Webpage = mergeInto,
                    };
                    mergeInto.Urls.Add(urlHistory);
                    session.Save(urlHistory);
                });

                return BatchJobExecutionResult.Success();
            }
        }

        protected override Task<BatchJobExecutionResult> OnExecuteAsync(CompleteMergeBatchJob batchJob)
        {
            throw new System.NotImplementedException();
        }
    }
}