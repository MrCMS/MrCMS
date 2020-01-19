using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IRoleManager
    {
        Task<IdentityResult> CreateAsync(Role role);
        Task UpdateNormalizedRoleNameAsync(Role role);
        Task<IdentityResult> UpdateAsync(Role role);
        Task<IdentityResult> DeleteAsync(Role role);
        Task<bool> RoleExistsAsync(string roleName);
        string NormalizeKey(string key);
        Task<Role> FindByIdAsync(string roleId);
        Task<string> GetRoleNameAsync(Role role);
        Task<IdentityResult> SetRoleNameAsync(Role role, string name);
        Task<string> GetRoleIdAsync(Role role);
        Task<Role> FindByNameAsync(string roleName);
        Task<IdentityResult> AddClaimAsync(Role role, Claim claim);
        Task<IdentityResult> RemoveClaimAsync(Role role, Claim claim);
        Task<IList<Claim>> GetClaimsAsync(Role role);
        void Dispose();
        IQueryable<Role> Roles { get; }
        bool SupportsQueryableRoles { get; }
        bool SupportsRoleClaims { get; }
    }
}