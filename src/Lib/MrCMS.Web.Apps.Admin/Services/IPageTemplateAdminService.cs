using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IPageTemplateAdminService
    {
        IPagedList<PageTemplate> Search(PageTemplateSearchQuery query);
        Task Add(AddPageTemplateModel model);
        Task<UpdatePageTemplateModel> GetEditModel(int id);
        Task Update(UpdatePageTemplateModel template);

        List<SelectListItem> GetPageTypeOptions();
        List<SelectListItem> GetLayoutOptions();
        Task<List<SelectListItem>> GetUrlGeneratorOptions(int templateId);
        List<SelectListItem> GetUrlGeneratorOptions(string pageType);
    }
}