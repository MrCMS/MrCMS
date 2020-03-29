using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class EditSiteBreadcrumb : Breadcrumb<SitesBreadcrumb>
    {
        private readonly IGlobalRepository<Site> _repository;

        public EditSiteBreadcrumb(IGlobalRepository<Site> repository)
        {
            _repository = repository;
        }
        public override string Controller => "Sites";
        public override string Action => "Edit";

        public override async Task Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            var site = await _repository.GetData(Id.Value);
            Name = site.Name;
        }
    }
}