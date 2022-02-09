using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserClaimManager
    {
        Task<IdentityResult> AddClaimAsync(User user, Claim claim);
        Task<IdentityResult> AddClaimsAsync(User user, IEnumerable<Claim> claims);
        Task<IdentityResult> ReplaceClaimAsync(User user, Claim claim, Claim newClaim);
        Task<IdentityResult> RemoveClaimAsync(User user, Claim claim);
        Task<IdentityResult> RemoveClaimsAsync(User user, IEnumerable<Claim> claims);
        Task<IList<Claim>> GetClaimsAsync(User user);
    }
}