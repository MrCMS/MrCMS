using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Admin.Services
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

        public async Task<List<SelectListItem>> GetOtherSiteOptions()
        {
            var sites = await _session.QueryOver<Site>().OrderBy(site => site.Name).Asc.Cacheable().ListAsync();
            return sites
                .BuildSelectItemList(site => site.DisplayName, site => site.Id.ToString(), emptyItem: new SelectListItem
                {
                    Text = "Do not copy",
                    Value = ""
                });
        }
    }
}