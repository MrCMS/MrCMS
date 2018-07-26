using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageParentAdminService
    {
        IEnumerable<SelectListItem> GetValidParents(Webpage webpage);
        void Set(Webpage webpage, int? parentId);
    }
}