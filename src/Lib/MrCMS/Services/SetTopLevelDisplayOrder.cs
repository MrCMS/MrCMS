using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Events;

namespace MrCMS.Services
{
    public abstract class SetTopLevelDisplayOrder<T> : IOnAdding<T> where T : Document
    {
        private readonly IGetDocumentsByParent<T> _getDocumentsByParent;

        public SetTopLevelDisplayOrder(IGetDocumentsByParent<T> getDocumentsByParent)
        {
            _getDocumentsByParent = getDocumentsByParent;
        }

        public void Execute(OnAddingArgs<T> args)
        {
            T tDoc = args.Item;
            // if the document isn't set or it's not top level (i.e. has a parent) we don't want to deal with it here
            if (tDoc == null || tDoc.Parent != null)
                return;

            // if it's not 0 it means it's been set, so we'll not update it
            if (tDoc.DisplayOrder != 0)
                return;

            var documentsByParent = _getDocumentsByParent.GetDocuments(null)
                .Where(doc => doc != tDoc).ToList();

            tDoc.DisplayOrder = documentsByParent.Any()
                ? documentsByParent.Max(category => category.DisplayOrder) + 1
                : 0;
        }
    }
}