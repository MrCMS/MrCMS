using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;
using System.Linq;

namespace MrCMS.Services
{
    public class RoleService : IRoleService
    {
        private readonly ISession _session;

        public RoleService(ISession session)
        {
            _session = session;
        }

        public void SaveRole(UserRole role)
        {
            _session.Transact(session => session.SaveOrUpdate(role));
        }

        public UserRole GetRole(int id)
        {
            return _session.Get<UserRole>(id);
        }

        public IEnumerable<UserRole> GetAllRoles()
        {
            return _session.QueryOver<UserRole>().Cacheable().List();
        }

        public UserRole GetRoleByName(string name)
        {
            return _session.QueryOver<UserRole>().Where(role => role.Name.IsLike(name, MatchMode.Exact)).Cacheable().
                                SingleOrDefault();
        }

        public void DeleteRole(UserRole role)
        {
            if (!role.IsAdmin)
                _session.Transact(session => session.Delete(role));
        }

        public bool IsOnlyAdmin(User user)
        {
            var adminRole = GetRoleByName(UserRole.Administrator);

            var users = adminRole.Users.Where(user1 => user1.IsActive).ToList();
            return users.Count() != 1 || users.First() != user;
        }
    }
}