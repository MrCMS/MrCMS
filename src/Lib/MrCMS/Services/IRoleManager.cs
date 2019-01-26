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
        Task<IdentityResult> CreateAsync(UserRole role);
        Task UpdateNormalizedRoleNameAsync(UserRole role);
        Task<IdentityResult> UpdateAsync(UserRole role);
        Task<IdentityResult> DeleteAsync(UserRole role);
        Task<bool> RoleExistsAsync(string roleName);
        string NormalizeKey(string key);
        Task<UserRole> FindByIdAsync(string roleId);
        Task<string> GetRoleNameAsync(UserRole role);
        Task<IdentityResult> SetRoleNameAsync(UserRole role, string name);
        Task<string> GetRoleIdAsync(UserRole role);
        Task<UserRole> FindByNameAsync(string roleName);
        Task<IdentityResult> AddClaimAsync(UserRole role, Claim claim);
        Task<IdentityResult> RemoveClaimAsync(UserRole role, Claim claim);
        Task<IList<Claim>> GetClaimsAsync(UserRole role);
        void Dispose();
        IQueryable<UserRole> Roles { get; }
        bool SupportsQueryableRoles { get; }
        bool SupportsRoleClaims { get; }
    }
}