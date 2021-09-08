using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.People;
using MrCMS.Models;
using X.PagedList;

namespace MrCMS.Services
{
    public interface IUserSearchService
    {
        Task<List<SelectListItem>> GetAllRoleOptions();
        Task<IPagedList<User>> GetUsersPaged(UserSearchQuery searchQuery);
    }
}