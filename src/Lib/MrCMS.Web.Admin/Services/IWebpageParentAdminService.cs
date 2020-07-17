using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Services
{
    public interface IWebpageParentAdminService
    {
        Webpage GetWebpage(int id);
        IEnumerable<SelectListItem> GetValidParents(Webpage webpage);
        void Set(int webpage, int? parentId);
    }
}