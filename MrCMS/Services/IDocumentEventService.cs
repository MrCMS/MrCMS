using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IDocumentEventService
    {
        void OnDocumentDeleted(Document document);
        void OnDocumentUnpublished(Document document);
        void OnDocumentAdded(Document document);
    }
}