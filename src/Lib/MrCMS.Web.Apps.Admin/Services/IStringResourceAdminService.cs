using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IStringResourceAdminService
    {
        Task<IPagedList<StringResource>> Search(StringResourceSearchQuery searchQuery);
        void Add(AddStringResourceModel resource);
        StringResource GetResource(int id);
        UpdateStringResourceModel GetEditModel(StringResource resource);
        void Update(UpdateStringResourceModel model);
        void Delete(int id);
        Task<List<SelectListItem>> GetLanguageOptions(string key, int? siteId);
        List<SelectListItem> SearchLanguageOptions();
        AddStringResourceModel GetNewResource(string key, int? id);
        List<SelectListItem> ChooseSiteOptions(ChooseSiteParams chooseSiteParams);
        List<SelectListItem> SearchSiteOptions();
    }
}