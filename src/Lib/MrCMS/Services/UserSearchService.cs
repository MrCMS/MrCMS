using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using NHibernate;
using NHibernate.Criterion;
using X.PagedList;

namespace MrCMS.Services
{
    public class UserSearchService : IUserSearchService
    {
        private readonly ISession _session;

        public UserSearchService(ISession session)
        {
            _session = session;
        }

        public async Task<List<SelectListItem>> GetAllRoleOptions()
        {
            var roles = await _session.QueryOver<UserRole>().OrderBy(role => role.Name).Asc.Cacheable().ListAsync();

            return roles.BuildSelectItemList(role => role.Name, role => role.Id.ToString(), emptyItemText: "Any role");
        }


        public async Task<IPagedList<User>> GetUsersPaged(UserSearchQuery searchQuery)
        {
            var query = _session.QueryOver<User>();

            if (!string.IsNullOrWhiteSpace(searchQuery.Email))
                query =
                    query.Where(
                        user =>
                            user.Email.IsInsensitiveLike(searchQuery.Email, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(searchQuery.FirstName))
                query =
                    query.Where(
                        user =>
                            user.FirstName.IsInsensitiveLike(searchQuery.FirstName, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(searchQuery.LastName))
                query =
                    query.Where(
                        user =>
                            user.LastName.IsInsensitiveLike(searchQuery.LastName, MatchMode.Anywhere));


            if (searchQuery.UserRoleId != null)
            {
                UserRole role = null;
                query = query.JoinAlias(user => user.Roles, () => role).Where(() => role.Id == searchQuery.UserRoleId);
            }

            query = query.OrderBy(x => x.Id).Desc;

            return await query.PagedAsync(searchQuery.Page);
        }
    }
}