using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using X.PagedList;

namespace MrCMS.Services
{
    public class UserSearchService : IUserSearchService
    {
        private readonly IDataReader _dataReader;

        public UserSearchService(IDataReader dataReader)
        {
            _dataReader = dataReader;
        }

        public List<SelectListItem> GetAllRoleOptions()
        {
            var roles = _dataReader.GlobalReadonly<UserRole>().OrderBy(x => x.Name).ToList();

            return roles.BuildSelectItemList(role => role.Name, role => role.Id.ToString(), emptyItemText: "Any role");
        }


        public IPagedList<User> GetUsersPaged(UserSearchQuery searchQuery)
        {
            var query = _dataReader.GlobalReadonly<User>();

            if (!string.IsNullOrWhiteSpace(searchQuery.Email))
                query =
                    query.Where(
                        user =>
                            EF.Functions.Like(user.Email, $"%{searchQuery.Email}%"));

            if (!string.IsNullOrWhiteSpace(searchQuery.FirstName))
                query =
                    query.Where(
                        user =>
                            EF.Functions.Like(user.FirstName, $"%{searchQuery.FirstName}%"));

            if (!string.IsNullOrWhiteSpace(searchQuery.LastName))
                query =
                    query.Where(
                        user =>
                            EF.Functions.Like(user.LastName, $"%{searchQuery.LastName}%"));


            if (searchQuery.UserRoleId != null)
            {
                query = query.Where(x => x.UserToRoles.Any(y => y.RoleId == searchQuery.UserRoleId));
            }

            return query.ToPagedList(searchQuery.Page);
        }
    }
}