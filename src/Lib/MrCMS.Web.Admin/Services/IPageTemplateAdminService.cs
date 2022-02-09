using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IPageTemplateAdminService
    {
        Task<IPagedList<PageTemplate>> Search(PageTemplateSearchQuery query);
        Task Add(AddPageTemplateModel model);
        Task<UpdatePageTemplateModel> GetEditModel(int id);
        Task Update(UpdatePageTemplateModel template);
        Task Delete(int id);

        List<SelectListItem> GetPageTypeOptions();
        Task<List<SelectListItem>> GetLayoutOptions();
        List<SelectListItem> GetUrlGeneratorOptions(string typeName);
    }
}