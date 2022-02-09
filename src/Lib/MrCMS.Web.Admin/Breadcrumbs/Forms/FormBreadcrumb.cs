using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Admin.Breadcrumbs.Forms
{
    public class FormBreadcrumb : ItemBreadcrumb<FormsBreadcrumb, Form>
    {
        public FormBreadcrumb(ISession session) : base(session)
        {
        }
    }
}