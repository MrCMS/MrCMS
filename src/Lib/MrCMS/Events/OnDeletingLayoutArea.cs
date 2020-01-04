using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Events
{
    public class OnDeletingLayoutArea : OnDataDeleting<LayoutArea>
    {
        public override Task<IResult> OnDeleting(LayoutArea entity, DbContext dbContext)
        {
            if (entity.Layout != null)
            {
                entity.Layout.LayoutAreas.Remove(entity); //required to clear cache
                dbContext.Update(entity.Layout);
            }

            return Success;
        }
    }
}