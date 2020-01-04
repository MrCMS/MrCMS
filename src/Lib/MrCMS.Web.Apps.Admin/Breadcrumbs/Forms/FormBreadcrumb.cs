using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Forms
{
    public class FormBreadcrumb : ItemBreadcrumb<FormsBreadcrumb, Form>
    {
        public FormBreadcrumb(IRepository<Form> session) : base(session)
        {
        }
    }
}