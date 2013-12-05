using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Services
{
    public interface IUserSearchService
    {
        List<SelectListItem> GetAllRoleOptions();
        IPagedList<User> GetUsersPaged(UserSearchQuery searchQuery);
    }
}