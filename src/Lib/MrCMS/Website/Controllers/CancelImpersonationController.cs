using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;

namespace MrCMS.Website.Controllers;

public class CancelImpersonationController : MrCMSUIController
{
    private readonly IUserImpersonationService _userImpersonationService;

    public CancelImpersonationController(IUserImpersonationService userImpersonationService)
    {
        _userImpersonationService = userImpersonationService;
    }

    [HttpPost]
    public async Task<RedirectResult> Cancel()
    {
        await _userImpersonationService.CancelImpersonation(User);
        return Redirect("/");
    }
}