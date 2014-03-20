using MrCMS.Entities.Documents;

namespace MrCMS.Events
{
    public interface IOnDocumentDeleted : IEvent<OnDocumentDeletedEventArgs>
    {
    }

    public class OnDocumentDeletedEventArgs
    {
        public OnDocumentDeletedEventArgs(Document document)
        {
            Document = document;
        }

        public Document Document { get; set; }
    }
}