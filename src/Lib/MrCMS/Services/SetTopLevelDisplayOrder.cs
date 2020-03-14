using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Events;

namespace MrCMS.Services
{
    public abstract class SetTopLevelDisplayOrder<T> : OnDataAdding<T> where T : Document
    {
        private readonly IGetDocumentsByParent<T> _getDocumentsByParent;

        public SetTopLevelDisplayOrder(IGetDocumentsByParent<T> getDocumentsByParent)
        {
            _getDocumentsByParent = getDocumentsByParent;
        }

        public override async Task<IResult> OnAdding(T entity, DbContext context)
        {
            // if the document isn't set or it's not top level (i.e. has a parent) we don't want to deal with it here
            if (entity == null || entity.ParentId != null)
                return await Success;

            // if it's not 0 it means it's been set, so we'll not update it
            if (entity.DisplayOrder != 0)
                return await Success;

            var documentsByParent = (await _getDocumentsByParent.GetDocuments(null))
                .Where(doc => doc != entity).ToList();

            entity.DisplayOrder = documentsByParent.Any()
                ? documentsByParent.Max(category => category.DisplayOrder) + 1
                : 0;
            return await Success;
        }
    }
}