using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class SetDocumentDisplayOrder : IOnAdding<Document>
    {
        private readonly IGetDocumentParents _getDocumentParents;

        public SetDocumentDisplayOrder(IGetDocumentParents getDocumentParents)
        {
            _getDocumentParents = getDocumentParents;
        }

        public void Execute(OnAddingArgs<Document> args)
        {
            Document document = args.Item;
            if (document != null && document.DisplayOrder == 0)
            {
                document.DisplayOrder = GetMaxParentDisplayOrder(document, args.Session);
            }
        }

        private int GetMaxParentDisplayOrder(Document document, ISession session)
        {
            if (document.Parent != null)
            {
                return session.QueryOver<Document>()
                    .Where(doc => doc.Parent.Id == document.Parent.Id)
                    .Select(Projections.Max<Document>(d => d.DisplayOrder))
                    .SingleOrDefault<int>();
            }
            if (document is MediaCategory)
            {
                List<MediaCategory> documentsByParent = _getDocumentParents.GetDocumentsByParent<MediaCategory>(null)
                    .Where(category => category != document).ToList();
                return documentsByParent.Any()
                    ? documentsByParent.Max(category => category.DisplayOrder) + 1
                    : 0;
            }
            if (document is Layout)
            {
                List<Layout> documentsByParent = _getDocumentParents.GetDocumentsByParent<Layout>(null)
                    .Where(layout => layout != document).ToList();
                return documentsByParent.Any()
                    ? documentsByParent.Max(category => category.DisplayOrder) + 1
                    : 0;
            }
            else
            {
                List<Webpage> documentsByParent = _getDocumentParents.GetDocumentsByParent<Webpage>(null)
                    .Where(webpage => webpage != document).ToList();
                return documentsByParent.Any()
                    ? documentsByParent.Max(category => category.DisplayOrder) + 1
                    : 0;
            }
        }
    }
}