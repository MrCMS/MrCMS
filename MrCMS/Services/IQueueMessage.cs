using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public interface IQueueMessage
    {
        void Queue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null, bool trySendImmediately = true);
    }
}