using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Apps.Admin.Models;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class SiteCloneOptionsAdminService : ISiteCloneOptionsAdminService
    {
        private readonly IGlobalRepository<Site> _repository;

        public SiteCloneOptionsAdminService(IGlobalRepository<Site> repository)
        {
            _repository = repository;
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
            return _repository.Readonly().OrderBy(x=>x.Name).ToList()
                .BuildSelectItemList(site => site.DisplayName, site => site.Id.ToString(), emptyItem: new SelectListItem
                {
                    Text = "Do not copy",
                    Value = ""
                });
        }
    }
}