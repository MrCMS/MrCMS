using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IUserAdminService
    {
        void AddUser(User user);
        UpdateUserModel GetUpdateModel(User user);
        User SaveUser(UpdateUserModel model, List<int> roles);
        void DeleteUser(int id);
        User GetUser(int id);
        bool IsUniqueEmail(string email, int? id);
        void SetPassword(int id, string password);
    }
}