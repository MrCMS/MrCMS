using System.Threading.Tasks;
using MrCMS.ACL.Rules;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Auth
{
    public interface IFrontEndEditingChecker
    {
        Task<bool> IsAllowed();
    }

    public class FrontEndEditingChecker : IFrontEndEditingChecker
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IAccessChecker _accessChecker;
        private readonly SiteSettings _siteSettings;

        public FrontEndEditingChecker(IGetCurrentUser getCurrentUser, IAccessChecker accessChecker, SiteSettings siteSettings)
        {
            _getCurrentUser = getCurrentUser;
            _accessChecker = accessChecker;
            _siteSettings = siteSettings;
        }

        public async Task<bool> IsAllowed()
        {
            if (!_siteSettings.EnableInlineEditing)
                return false;
            var user = await _getCurrentUser.Get();
            return user != null && await _accessChecker.CanAccess<AdminBarACL>(AdminBarACL.Show, user);
        }
    }
}