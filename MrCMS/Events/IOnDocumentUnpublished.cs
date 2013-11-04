using MrCMS.Entities.Documents;

namespace MrCMS.Events
{
    public interface IOnDocumentUnpublished
    {
        void OnDocumentUnpublished(Document document);
    }
}