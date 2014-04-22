using MrCMS.Entities.Documents;
using MrCMS.Entities.People;

namespace MrCMS.Events.Documents
{
    public class OnDocumentUpdatedEventArgs
    {
        public OnDocumentUpdatedEventArgs(Document document, User user)
        {
            Document = document;
            User = user;
        }

        public Document Document { get; set; }
        public User User { get; set; }
    }
}