using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events
{
    public class OnDeletingLayout : OnDataDeleting<Layout>
    {
        public override Task<IResult> OnDeleting(Layout entity, DbContext dbContext)
        {
            foreach (var pageTemplate in dbContext.Set<PageTemplate>().Where(x => x.LayoutId == entity.Id))
            {
                pageTemplate.LayoutId = null;
                dbContext.Update(pageTemplate);
            }
            return Success;
        }
    }
}