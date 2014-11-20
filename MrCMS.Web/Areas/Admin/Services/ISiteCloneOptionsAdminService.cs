using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.CloneSite;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ISiteCloneOptionsAdminService
    {
        List<SiteCloneOption> GetClonePartOptions();
        List<SelectListItem> GetOtherSiteOptions();
    }

    public class SiteCloneOptionsAdminService : ISiteCloneOptionsAdminService
    {
        private readonly ISession _session;

        public SiteCloneOptionsAdminService(ISession session)
        {
            _session = session;
        }

        public List<SiteCloneOption> GetClonePartOptions()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<ICloneSiteParts>()
                .OrderBy(CloneSiteExtensions.GetCloneSitePartOrder)
                .Select(type => new SiteCloneOption
                {
                    TypeName = type.FullName,
                    DisplayName = CloneSiteExtensions.GetCloneSitePartDisplayName(type)
                }).ToList();
        }

        public List<SelectListItem> GetOtherSiteOptions()
        {
            return _session.QueryOver<Site>().OrderBy(site => site.Name).Asc.Cacheable().List()
                .BuildSelectItemList(site => site.DisplayName, site => site.Id.ToString(), emptyItem: new SelectListItem
                {
                    Text = "Do not copy",
                    Value = ""
                });
        }
    }

    public class SiteCloneOption
    {
        public string DisplayName { get; set; }
        public string TypeName { get; set; }
    }
}