using MrCMS.Entities.Multisite;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class EditSiteBreadcrumb : Breadcrumb<SitesBreadcrumb>
    {
        private readonly ISession _session;

        public EditSiteBreadcrumb(ISession session)
        {
            _session = session;
        }
        public override string Controller => "Sites";
        public override string Action => "Edit";

        public override void Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            var site = _session.Get<Site>(Id);
            Name = site.Name;
        }
    }
}