using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Forms
{
    public class FormBreadcrumb : ItemBreadcrumb<FormsBreadcrumb, Form>
    {
        public FormBreadcrumb(IRepository<Form> session) : base(session)
        {
        }
    }
}