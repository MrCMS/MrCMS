using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;

namespace MrCMS.Website.Controllers;

public class CancelImpersonationController : MrCMSUIController
{
    private IUserImpersonationService _userImpersonationService;

    public CancelImpersonationController(IUserImpersonationService userImpersonationService)
    {
        _userImpersonationService = userImpersonationService;
    }

    [HttpPost]
    public RedirectResult Cancel()
    {
        _userImpersonationService.CancelImpersonation();
        return Redirect("/");
    }
}