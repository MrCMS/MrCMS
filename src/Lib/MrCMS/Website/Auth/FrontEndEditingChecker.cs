using System.Threading.Tasks;
using MrCMS.ACL.Rules;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Auth;

public class FrontEndEditingChecker : IFrontEndEditingChecker
{
    private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;
    private readonly IAccessChecker _accessChecker;
    private readonly SiteSettings _siteSettings;

    public FrontEndEditingChecker(IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal, IAccessChecker accessChecker, SiteSettings siteSettings)
    {
        _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
        _accessChecker = accessChecker;
        _siteSettings = siteSettings;
    }

    public async Task<bool> IsAllowed()
    {
        if (!_siteSettings.EnableInlineEditing)
            return false;
        var user = await _getCurrentClaimsPrincipal.GetPrincipal();
        return user != null && await _accessChecker.CanAccess<AdminBarACL>(AdminBarACL.Show, user);
    }
}