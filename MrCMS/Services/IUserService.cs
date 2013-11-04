using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Paging;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public interface IUserService
    {
        void AddUser(User user);
        void SaveUser(User user);
        User GetUser(int id);
        IList<User> GetAllUsers();
        IPagedList<User> GetUsersPaged(UserSearchQuery searchQuery);
        User GetUserByEmail(string email);
        User GetUserByResetGuid(Guid resetGuid);
        User GetCurrentUser(HttpContextBase context);
        void DeleteUser(User user);
        bool IsUniqueEmail(string email, int? id = null);
        int ActiveUsers();
        int NonActiveUsers();

        T Get<T>(User user) where T : SystemEntity, IBelongToUser;
        IList<T> GetAll<T>(User user) where T : SystemEntity, IBelongToUser;
        IPagedList<T> GetPaged<T>(User user, QueryOver<T> query = null, int page = 1) where T : SystemEntity, IBelongToUser;
    }
}