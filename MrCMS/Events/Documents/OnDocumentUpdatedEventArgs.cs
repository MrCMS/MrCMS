using MrCMS.Entities.Documents;

namespace MrCMS.Events.Documents
{
    public class OnDocumentUpdatedEventArgs
    {
        public OnDocumentUpdatedEventArgs(Document document, string action = null)
        {
            Document = document;
            Action = action ?? "updated";
        }

        public Document Document { get; private set; }
        public string Action { get; private set; }
    }
}