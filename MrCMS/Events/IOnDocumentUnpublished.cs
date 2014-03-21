using MrCMS.Entities.Documents;

namespace MrCMS.Events
{
    public interface IOnDocumentUnpublished : IEvent<OnDocumentUnpublishedEventArgs>
    {
    }

    public class OnDocumentUnpublishedEventArgs
    {
        public OnDocumentUnpublishedEventArgs(Document document)
        {
            Document = document;
        }

        public Document Document { get; set; }
    }
}