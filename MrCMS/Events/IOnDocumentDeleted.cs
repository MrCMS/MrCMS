using MrCMS.Entities.Documents;

namespace MrCMS.Events
{
    public interface IOnDocumentDeleted
    {
        void OnDocumentDeleted(Document document);
    }
}