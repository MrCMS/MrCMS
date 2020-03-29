using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserLookup
    {
        User GetUserByEmail(string email);
        Task<User>  GetUserByResetGuid(Guid resetGuid);
        User GetUserByGuid(Guid guid);
        User GetUserById(int id);
        User GetCurrentUser(HttpContext context);
        User GetCurrentUser(IPrincipal principal);
    }
}