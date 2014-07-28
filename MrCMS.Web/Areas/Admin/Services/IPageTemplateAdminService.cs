using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IPageTemplateAdminService
    {
        IPagedList<PageTemplate> Search(PageTemplateSearchQuery query);
        void Add(PageTemplate template);
        void Update(PageTemplate template);

        List<SelectListItem> GetPageTypeOptions();
        List<SelectListItem> GetLayoutOptions();
        List<SelectListItem> GetUrlGeneratorOptions(Type type);
    }
}