using MrCMS.Entities.Documents;

namespace MrCMS.Events.Documents
{
    public interface IOnDocumentAdded : IEvent<OnDocumentAddedEventArgs>
    {
    }

    public class OnDocumentAddedEventArgs
    {
        public OnDocumentAddedEventArgs(Document document)
        {
            Document = document;
        }

        public Document Document { get; set; }
    }
}