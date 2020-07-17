using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IPageTemplateAdminService
    {
        IPagedList<PageTemplate> Search(PageTemplateSearchQuery query);
        void Add(AddPageTemplateModel model);
        UpdatePageTemplateModel GetEditModel(int id);
        void Update(UpdatePageTemplateModel template);

        List<SelectListItem> GetPageTypeOptions();
        List<SelectListItem> GetLayoutOptions();
        List<SelectListItem> GetUrlGeneratorOptions(string typeName);
    }
}