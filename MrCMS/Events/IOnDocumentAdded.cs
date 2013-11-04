using MrCMS.Entities.Documents;

namespace MrCMS.Events
{
    public interface IOnDocumentAdded
    {
        void OnDocumentAdded(Document document);
    }
}