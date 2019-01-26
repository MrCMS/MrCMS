using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.People;
using MrCMS.Models;
using X.PagedList;

namespace MrCMS.Services
{
    public interface IUserSearchService
    {
        List<SelectListItem> GetAllRoleOptions();
        IPagedList<User> GetUsersPaged(UserSearchQuery searchQuery);
    }
}