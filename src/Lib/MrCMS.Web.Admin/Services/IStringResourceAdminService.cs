using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IStringResourceAdminService
    {
        IPagedList<StringResource> Search(StringResourceSearchQuery searchQuery);
        void Add(AddStringResourceModel resource);
        StringResource GetResource(int id);
        UpdateStringResourceModel GetEditModel(StringResource resource);
        void Update(UpdateStringResourceModel model);
        void Delete(int id);
        List<SelectListItem> GetLanguageOptions(string key, int? siteId);
        List<SelectListItem> SearchLanguageOptions();
        AddStringResourceModel GetNewResource(string key, int? id);
        List<SelectListItem> ChooseSiteOptions(ChooseSiteParams chooseSiteParams);
        List<SelectListItem> SearchSiteOptions();
    }
}