using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class RoleStore : IRoleStore<UserRole>
    {
        private readonly IRoleService _roleService;

        public RoleStore(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public void Dispose()
        {
            
        }

        public Task CreateAsync(UserRole role)
        {
            return Task.Run(() => _roleService.SaveRole(role));
        }

        public Task UpdateAsync(UserRole role)
        {
            return Task.Run(() => _roleService.SaveRole(role));
        }

        public Task DeleteAsync(UserRole role)
        {
            return Task.Run(() => _roleService.DeleteRole(role));
        }

        public Task<UserRole> FindByIdAsync(string roleId)
        {
            return Task.Run(() =>
                {
                    int id;
                    int.TryParse(roleId, out id);
                    return _roleService.GetRole(id);
                });
        }

        public Task<UserRole> FindByNameAsync(string roleName)
        {
            return Task.Run(() => _roleService.GetRoleByName(roleName));
        }
    }
}