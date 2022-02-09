using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using NHibernate;
using NHibernate.Criterion;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class RoleService : IRoleService
    {
        private readonly ISession _session;

        public RoleService(ISession session)
        {
            _session = session;
        }

        public async Task SaveRole(UserRole role)
        {
            await _session.TransactAsync(session => session.SaveOrUpdateAsync(role));
        }

        public async Task<IList<UserRole>> GetAllRoles()
        {
            return await _session.QueryOver<UserRole>().Cacheable().ListAsync();
        }

        public async Task<UserRole> GetRoleByName(string name)
        {
            return await _session.QueryOver<UserRole>().Where(role => role.Name.IsLike(name, MatchMode.Exact))
                .Cacheable().SingleOrDefaultAsync();
        }

        public async Task DeleteRole(UserRole role)
        {
            if (!role.IsAdmin)
                await _session.TransactAsync(session => session.DeleteAsync(role));
        }

        public async Task<bool> IsOnlyAdmin(User user)
        {
            var adminRole = await GetRoleByName(UserRole.Administrator);
            var users = _session.Query<User>()
                .Where(x => x.Roles.Any(role => role.Id == adminRole.Id) && x.IsActive)
                .WithOptions(x => x.SetCacheable(true)).Distinct().Count();
            return users == 1;
        }

        public async Task<IEnumerable<AutoCompleteResult>> Search(string term)
        {
            var userRoles = await _session.QueryOver<UserRole>()
                .Where(x => x.Name.IsInsensitiveLike(term, MatchMode.Start))
                .ListAsync();
            return
                userRoles.Select(
                    tag =>
                        new AutoCompleteResult
                        {
                            id = tag.Id,
                            label = tag.Name,
                            value = tag.Name
                        });
        }

        public Task<UserRole> GetRole(int id)
        {
            return _session.GetAsync<UserRole>(id);
        }
    }
}