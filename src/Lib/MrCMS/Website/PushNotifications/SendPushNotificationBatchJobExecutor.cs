using System.Threading;
using System.Threading.Tasks;
using MrCMS.Batching;
using Newtonsoft.Json;

namespace MrCMS.Website.PushNotifications
{
    public class SendPushNotificationBatchJobExecutor : BaseBatchJobExecutor<SendPushNotificationBatchJob>
    {
        private readonly ISendPushNotification _sendPushNotification;

        public SendPushNotificationBatchJobExecutor(ISendPushNotification sendPushNotification)
        {
            _sendPushNotification = sendPushNotification;
        }

        protected override async Task<BatchJobExecutionResult> OnExecuteAsync(SendPushNotificationBatchJob batchJob,
            CancellationToken token)
        {
            var data = JsonConvert.DeserializeObject<SendPushNotificationData>(batchJob.Data);
            var result = await _sendPushNotification.SendNotification(data);

            return result.StatusCode.HasValue
                ? BatchJobExecutionResult.Failure(result.Error)
                : BatchJobExecutionResult.Success();
        }
    }
} 