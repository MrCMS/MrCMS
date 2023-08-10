using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Services.Auth;

namespace MrCMS.Website.Controllers;

public class CancelImpersonationController : MrCMSUIController
{
    private readonly IUserImpersonationService _userImpersonationService;
    private readonly ISignInManager _signInManager;

    public CancelImpersonationController(IUserImpersonationService userImpersonationService,
        ISignInManager signInManager)
    {
        _userImpersonationService = userImpersonationService;
        _signInManager = signInManager;
    }

    [HttpPost]
    public async Task<RedirectResult> Cancel()
    {
        var loggedInUser = await _userImpersonationService.CancelImpersonation(User);
        // refresh current user's claims
        await _signInManager.RefreshSignInAsync(loggedInUser);
        return Redirect("/");
    }
}