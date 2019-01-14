using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Forms
{
    public class FormBreadcrumb : ItemBreadcrumb<FormsBreadcrumb, Form>
    {
        public FormBreadcrumb(ISession session) : base(session)
        {
        }
    }
}