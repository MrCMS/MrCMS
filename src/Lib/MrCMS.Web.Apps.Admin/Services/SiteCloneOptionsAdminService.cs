using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Apps.Admin.Models;


namespace MrCMS.Web.Apps.Admin.Services
{
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
}