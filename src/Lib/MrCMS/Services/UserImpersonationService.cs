using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services.Auth;
using MrCMS.Website.Auth;

namespace MrCMS.Services;

public class UserImpersonationService : IUserImpersonationService, IOnLoggedOut
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAccessChecker _accessChecker;

    private readonly IUserClaimManager _userClaimManager;
    private readonly IUserStore<User> _userStore;
    public const string UserImpersonationId = "IsCurrentlyImpersonating.Id";
    public const string UserImpersonationName = "IsCurrentlyImpersonating.Name";
    public static readonly string[] ImpersonationKeys = { UserImpersonationId, UserImpersonationName };

    public UserImpersonationService(IHttpContextAccessor contextAccessor, IAccessChecker accessChecker,
        IUserClaimManager userClaimManager, IUserStore<User> userStore)
    {
        _contextAccessor = contextAccessor;
        _accessChecker = accessChecker;
        _userClaimManager = userClaimManager;
        _userStore = userStore;
    }

    public async Task<UserImpersonationResult> Impersonate(ClaimsPrincipal currentPrincipal, User user)
    {
        if (user == null)
            return new UserImpersonationResult { Error = "User not found" };


        var identity = currentPrincipal.Identity as ClaimsIdentity;
        if (identity == null)
            return new UserImpersonationResult { Error = "Must be logged in to impersonate" };
        var currentUser = await _userStore.FindByNameAsync(identity.Name, default);
        if (currentUser == null)
            return new UserImpersonationResult { Error = "Can only impersonate when logged in" };

        var canAccess = await _accessChecker.CanAccess<UserACL>(UserACL.Impersonate, currentUser);

        if (!canAccess)
        {
            return new UserImpersonationResult { Error = "Only admins can impersonate another user" };
        }

        if (user.Id == currentUser.Id)
            return new UserImpersonationResult { Error = "Cannot impersonate self" };

        if (user.IsAdmin)
            return new UserImpersonationResult { Error = "Cannot impersonate an admin" };

        var idClaim = new Claim(UserImpersonationId, user.Id.ToString());
        var nameClaim = new Claim(UserImpersonationName, user.Email);

        // add to db
        await _userClaimManager.AddClaimAsync(currentUser, idClaim);
        await _userClaimManager.AddClaimAsync(currentUser, nameClaim);

        return new UserImpersonationResult();
    }

    // private async Task<User> GetCurrentUserFromContext(HttpContext httpContext)
    // {
    //     var currentUser = await _userStore.FindByNameAsync(httpContext.User.Identity?.Name, default);
    //     return currentUser;
    // }

    public async Task<User> GetCurrentlyImpersonatedUser(ClaimsPrincipal principal)
    {
        // get the impersonation claim for the user
        var (id, name) = GetImpersonationClaims(principal);

        // if they don't have one, return null
        if (id == null)
            return null;

        // get the user id from the claim
        var impersonatedUserId = id.Value;

        // get the user by that id
        return await _userStore.FindByIdAsync(impersonatedUserId, default);
    }


    public async Task CancelImpersonation(ClaimsPrincipal principal)
    {
        // var 
        // var currentUser = await GetCurrentUserFromContext(httpContext);
        // if (currentUser == null)
        //     return;
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null)
            return;

        var (id, name) = GetImpersonationClaims(principal);
        var currentUser = await _userStore.FindByNameAsync(identity.Name, default);
        if (currentUser == null)
            return;

        // we want to remove the one with the user impersonation key if it exists 
        if (id != null)
        {
            identity.TryRemoveClaim(id);
            await _userClaimManager.RemoveClaimAsync(currentUser, id);
        }

        if (name != null)
        {
            identity.TryRemoveClaim(name);
            await _userClaimManager.RemoveClaimAsync(currentUser, name);
        }
    }

    private static (Claim id, Claim name) GetImpersonationClaims(ClaimsPrincipal user)
    {
        var existingClaims = user.Claims.ToList();
        var id = existingClaims.FirstOrDefault(x => x.Type == UserImpersonationId);
        var name = existingClaims.FirstOrDefault(x => x.Type == UserImpersonationName);
        return (id, name);
    }

    public async Task Execute(LoggedOutEventArgs args)
    {
        await CancelImpersonation(args.User);
    }
}