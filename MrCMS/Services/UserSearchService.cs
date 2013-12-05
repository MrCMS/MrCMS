using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class UserSearchService : IUserSearchService
    {
        private readonly ISession _session;

        public UserSearchService(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetAllRoleOptions()
        {
            var roles = _session.QueryOver<UserRole>().OrderBy(role => role.Name).Asc.Cacheable().List();

            return roles.BuildSelectItemList(role => role.Name, role => role.Id.ToString(), emptyItemText: "Any role");
        }


        public IPagedList<User> GetUsersPaged(UserSearchQuery searchQuery)
        {
            var query = _session.QueryOver<User>();

            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
                query =
                    query.Where(
                        user =>
                        user.Email.IsInsensitiveLike(searchQuery.Query, MatchMode.Anywhere) ||
                        user.LastName.IsInsensitiveLike(searchQuery.Query, MatchMode.Anywhere) ||
                        user.FirstName.IsInsensitiveLike(searchQuery.Query, MatchMode.Anywhere));
            if (searchQuery.UserRoleId != null)
            {
                UserRole role = null;
                query = query.JoinAlias(user => user.Roles, () => role).Where(() => role.Id == searchQuery.UserRoleId);
            }

            return query.Paged(searchQuery.Page);
        }
    }
}