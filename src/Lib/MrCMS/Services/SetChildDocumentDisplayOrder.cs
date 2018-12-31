using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Events;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class SetChildDocumentDisplayOrder : IOnAdding<Document>
    {
        public void Execute(OnAddingArgs<Document> args)
        {
            Document document = args.Item;
            // if the document isn't set or it's top level (i.e. no parent) we don't want to deal with it here
            if (document?.Parent == null)
                return;

            // if it's not 0 it means it's been set, so we'll not update it
            if (document.DisplayOrder != 0)
                return;

            document.DisplayOrder = GetMaxParentDisplayOrder(document.Parent, args.Session);
        }

        private int GetMaxParentDisplayOrder(Document parent, ISession session)
        {
            return session.QueryOver<Document>()
                .Where(doc => doc.Parent.Id == parent.Id)
                .Select(Projections.Max<Document>(d => d.DisplayOrder))
                .SingleOrDefault<int>();
        }
    }
}