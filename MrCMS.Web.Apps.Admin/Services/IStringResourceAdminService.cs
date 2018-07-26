using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IStringResourceAdminService
    {
        IPagedList<StringResource> Search(StringResourceSearchQuery searchQuery);
        void Add(StringResource resource);
        void Update(StringResource resource);
        void Delete(StringResource resource);
        List<SelectListItem> GetLanguageOptions(string key, Site site);
        List<SelectListItem> SearchLanguageOptions();
        StringResource GetNewResource(string key, Site site);
        List<SelectListItem> ChooseSiteOptions(ChooseSiteParams chooseSiteParams);
        List<SelectListItem> SearchSiteOptions();
    }
}