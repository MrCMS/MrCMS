using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Events;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class DocumentEventService : IDocumentEventService
    {
        private readonly IEnumerable<IOnDocumentDeleted> _onDocumentDeleteds;
        private readonly IEnumerable<IOnDocumentUnpublished> _onDocumentUnpublisheds;
        private readonly IEnumerable<IOnDocumentAdded> _onDocumentAddeds;

        public DocumentEventService(IEnumerable<IOnDocumentDeleted> onDocumentDeleteds,
                                    IEnumerable<IOnDocumentUnpublished> onDocumentUnpublisheds,
                                    IEnumerable<IOnDocumentAdded> onDocumentAddeds)
        {
            _onDocumentDeleteds = onDocumentDeleteds;
            _onDocumentUnpublisheds = onDocumentUnpublisheds;
            _onDocumentAddeds = onDocumentAddeds;
        }

        public void OnDocumentDeleted(Document document)
        {
            _onDocumentDeleteds.ForEach(deleted => deleted.OnDocumentDeleted(document));
        }

        public void OnDocumentUnpublished(Document document)
        {
            _onDocumentUnpublisheds.ForEach(deleted => deleted.OnDocumentUnpublished(document));
        }

        public void OnDocumentAdded(Document document)
        {
            _onDocumentAddeds.ForEach(added => added.OnDocumentAdded(document));
        }
    }
}