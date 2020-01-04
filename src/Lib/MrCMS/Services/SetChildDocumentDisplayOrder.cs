using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Events;

namespace MrCMS.Services
{
    public class SetChildDocumentDisplayOrder : OnDataAdding<Document>
    {
        private int GetMaxParentDisplayOrder(Document parent, DbContext context)
        {
            return context.Set<Document>()
                .Where(doc => doc.Parent.Id == parent.Id)
                .Max(x => x.DisplayOrder);
        }

        public override Task<IResult> OnAdding(Document entity, DbContext context)
        {
            // if the document isn't set or it's top level (i.e. no parent) we don't want to deal with it here
            if (entity?.Parent == null)
                return Success;

            // if it's not 0 it means it's been set, so we'll not update it
            if (entity.DisplayOrder != 0)
                return Success;

            entity.DisplayOrder = GetMaxParentDisplayOrder(entity.Parent, context);
            return Success;
        }
    }
}