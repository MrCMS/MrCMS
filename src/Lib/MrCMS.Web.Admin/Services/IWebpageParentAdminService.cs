using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Services
{
    public interface IWebpageParentAdminService
    {
        Task<Webpage> GetWebpage(int id);
        Task<IEnumerable<SelectListItem>> GetValidParents(Webpage webpage);
        Task Set(int webpage, int? parentId);
    }
}