using MrCMS.Entities.Interaction;
using MrCMS.Entities.Messaging;

namespace MrCMS.Services
{
    public interface IInteractionService
    {
        void AddContactUs(ContactUs contactUs);
        void SendNotificationMessage(QueuedMessage queuedMessage);
    }
}