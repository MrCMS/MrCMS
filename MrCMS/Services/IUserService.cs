using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IUserService
    {
        void SaveUser(User user);
        User GetUser(int id);
        IEnumerable<User> GetAllUsers();
        User GetUserByEmail(string email);
        User GetUserByResetGuid(Guid resetGuid);
        User GetCurrentUser(HttpContextBase context);
        User GetCurrentUser(HttpContext context);
        void SetPassword(int userId, string password);
        void SaveRole(UserRole role);
        UserRole GetRole(int id);
        IEnumerable<UserRole> GetAllRoles();
        UserRole GetRoleByName(string name);
        void DeleteRole(UserRole role);
    }
}