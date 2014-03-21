using MrCMS.Entities.Documents;

namespace MrCMS.Events.Documents
{
    public class OnDocumentUpdatedEventArgs
    {
        public OnDocumentUpdatedEventArgs(Document document)
        {
            Document = document;
        }

        public Document Document { get; set; }
    }
}