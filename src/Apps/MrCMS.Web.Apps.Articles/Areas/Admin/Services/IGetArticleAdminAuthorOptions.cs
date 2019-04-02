using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public interface IGetArticleAdminAuthorOptions
    {
        IList<SelectListItem> GetUsers();
    }
}