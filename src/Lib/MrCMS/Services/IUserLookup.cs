using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserLookup
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByResetGuid(Guid resetGuid);
        Task<User> GetUserByGuid(Guid guid);
        Task<User> GetUserById(int id);
        Task<User> GetCurrentUser(HttpContext context);
        Task<User> GetCurrentUser(IPrincipal principal);
    }
}