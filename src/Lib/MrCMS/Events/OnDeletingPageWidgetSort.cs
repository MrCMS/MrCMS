using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events
{
    public class OnDeletingPageWidgetSort : OnDataDeleting<PageWidgetSort>
    {
        public override Task<IResult> OnDeleting(PageWidgetSort entity, DbContext dbContext)
        {
            if (entity.LayoutArea.PageWidgetSorts.Contains(entity))
                entity.LayoutArea.PageWidgetSorts.Remove(entity);
            if (entity.Webpage.PageWidgetSorts.Contains(entity))
                entity.Webpage.PageWidgetSorts.Remove(entity);
            if (entity.Widget.PageWidgetSorts.Contains(entity))
                entity.Widget.PageWidgetSorts.Remove(entity);

            return Success;
        }
    }
}