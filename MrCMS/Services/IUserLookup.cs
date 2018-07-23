using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserLookup
    {
        User GetUserByEmail(string email);
        User GetUserByResetGuid(Guid resetGuid);
        User GetUserByGuid(Guid guid);
        User GetCurrentUser(HttpContext context);
    }
}