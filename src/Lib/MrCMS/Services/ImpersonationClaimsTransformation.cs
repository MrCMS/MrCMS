using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Website.Auth;

namespace MrCMS.Services;

public class ImpersonationClaimsTransformation : IClaimsTransformation
{
    private readonly IUserClaimManager _userClaimStore;
    private readonly IUserLookup _userLookup;
    private readonly IPerformAclCheck _performAclCheck;

    public ImpersonationClaimsTransformation(IUserClaimManager userClaimStore, IUserLookup userLookup,
        IPerformAclCheck performAclCheck)
    {
        _userClaimStore = userClaimStore;
        _userLookup = userLookup;
        _performAclCheck = performAclCheck;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // if they're not logged in, we're done
        if (principal.Identity?.IsAuthenticated != true)
            return principal;

        var user = await _userLookup.GetUserByEmail(principal.Identity.Name);
        
        // clone the principal to avoid modifying the original
        var claimsPrincipal = principal.Clone();
        var newIdentity = (claimsPrincipal.Identity as ClaimsIdentity)!;
        
        // remove any existing impersonation claims
        var canImpersonate =
            _performAclCheck.CanAccessLogic<UserACL>(user.Roles.Select(x => x.Name), UserACL.Impersonate);
        var existingClaims = newIdentity.FindAll(x => UserImpersonationService.ImpersonationKeys.Contains(x.Type)).ToList();
        
        foreach (var claim in existingClaims)
        {
            newIdentity.RemoveClaim(claim);
        }

        // if they can't impersonate anyone, we're done
        if (!canImpersonate)
        {
            return claimsPrincipal;
        }

        // load claims from the db and set them
        var newClaims = await _userClaimStore.GetClaimsAsync(user);
        foreach (var claim in newClaims)
        {
            newIdentity.AddClaim(claim);
        }

        return claimsPrincipal;
    }

    private bool HasImpersonationClaims(ClaimsPrincipal principal)
    {
        return principal.Claims.Any(x => UserImpersonationService.ImpersonationKeys.Contains(x.Type));
    }
}