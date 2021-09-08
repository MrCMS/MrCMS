using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public interface IGetArticleAdminAuthorOptions
    {
        Task<IList<SelectListItem>> GetUsers();
    }
}