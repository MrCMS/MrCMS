using System;
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
            foreach (var pageTemplate in entity.PageTemplates)
            {
                pageTemplate.Layout = null;
            }
            entity.PageTemplates.Clear();
            return Success;
        }
    }
}