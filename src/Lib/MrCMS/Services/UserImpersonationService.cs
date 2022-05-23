using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services.Auth;
using MrCMS.Website.Auth;

namespace MrCMS.Services;

public class UserImpersonationService : IUserImpersonationService,IOnLoggedOut
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAccessChecker _accessChecker;
    private readonly IGetCurrentUser _getCurrentUser;
    public const string UserImpersonationKey = nameof(UserImpersonationKey);

    public UserImpersonationService(IHttpContextAccessor contextAccessor, IAccessChecker accessChecker,
        IGetCurrentUser getCurrentUser)
    {
        _contextAccessor = contextAccessor;
        _accessChecker = accessChecker;
        _getCurrentUser = getCurrentUser;
    }

    public async Task<UserImpersonationResult> Impersonate(User user)
    {
        if (user == null)
            return new UserImpersonationResult { Error = "User not found" };
        
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
            return new UserImpersonationResult { Error = "Can only impersonate in a web request" };

        var currentUser = await _getCurrentUser.Get();
        var canAccess = await _accessChecker.CanAccess<UserACL>(UserACL.Impersonate, currentUser);

        if (!canAccess)
        {
            return new UserImpersonationResult { Error = "Only admins can impersonate another user" };
        }

        if (user.Id == currentUser.Id)
            return new UserImpersonationResult { Error = "Cannot impersonate self" };

        if (user.IsAdmin)
            return new UserImpersonationResult { Error = "Cannot impersonate an admin" };

        httpContext.Session.SetInt32(UserImpersonationKey, user.Id);

        return new UserImpersonationResult();
    }


    public void CancelImpersonation()
    {
        _contextAccessor.HttpContext?.Session.Remove(UserImpersonationKey);
    }

    public Task Execute(LoggedOutEventArgs args)
    {
        CancelImpersonation();
        return Task.CompletedTask;
    }
}