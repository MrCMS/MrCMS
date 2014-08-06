using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWebpageParentAdminService
    {
        IEnumerable<SelectListItem> GetValidParents(Webpage webpage);
        void Set(Webpage webpage, int? parentId);
    }
}