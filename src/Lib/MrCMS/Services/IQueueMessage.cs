using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public interface IQueueMessage
    {
        Task Queue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null, bool trySendImmediately = true);
    }
}